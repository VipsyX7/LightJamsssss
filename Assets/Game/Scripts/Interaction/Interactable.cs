using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LightJam
{
    /// <summary>
    /// 靠近后按 E 触发的可交互物。可配置对话、给予物品、解锁条件。
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        static readonly List<Interactable> Active = new List<Interactable>();

        [Header("提示")]
        public string prompt = "按 E 查看";
        public float radius = 1.7f;

        [Header("对话")]
        public string speaker = "你";
        [TextArea(2, 5)] public string[] lines;
        public DialogueSequence sequence;

        [Header("解密")]
        [Tooltip("获得的物品 ID，例如 brass_key。")]
        public string giveItemId;
        [Tooltip("需要已拥有的物品 ID。")]
        public string requireItemId;
        [TextArea(2, 4)] public string[] missingItemLines;
        [TextArea(2, 4)] public string[] usedLines;
        public bool consumeRequiredItem;
        public bool hideAfterSuccess;

        [Header("事件")]
        public UnityEvent onInteractSuccess = new UnityEvent();

        bool used;

        public bool CanInteract => isActiveAndEnabled;

        void Awake()
        {
            if (onInteractSuccess == null)
                onInteractSuccess = new UnityEvent();
        }

        public void AddSuccessListener(UnityAction callback)
        {
            if (callback == null)
                return;
            if (onInteractSuccess == null)
                onInteractSuccess = new UnityEvent();
            onInteractSuccess.AddListener(callback);
        }

        void OnEnable()
        {
            if (!Active.Contains(this))
                Active.Add(this);
        }

        void OnDisable()
        {
            Active.Remove(this);
        }

        public static Interactable FindNearest(Vector2 position, Collider2D fromCollider = null, float extraRange = 0f)
        {
            Interactable best = null;
            float bestDist = float.MaxValue;

            for (int i = 0; i < Active.Count; i++)
            {
                var item = Active[i];
                if (item == null || !item.CanInteract)
                    continue;

                float dist = DistanceTo(item, position, fromCollider);
                if (dist <= item.radius + extraRange && dist < bestDist)
                {
                    best = item;
                    bestDist = dist;
                }
            }

            return best;
        }

        static float DistanceTo(Interactable item, Vector2 position, Collider2D fromCollider)
        {
            var target = item.GetComponent<Collider2D>();
            if (fromCollider != null && target != null && fromCollider.enabled && target.enabled)
            {
                ColliderDistance2D result = Physics2D.Distance(fromCollider, target);
                return Mathf.Max(0f, result.distance);
            }

            if (target != null && target.enabled)
                return Vector2.Distance(position, target.ClosestPoint(position));

            return Vector2.Distance(position, item.transform.position);
        }

        public void Interact()
        {
            if (used && usedLines != null && usedLines.Length > 0)
            {
                DialogueUI.Ensure().Play(speaker, usedLines);
                return;
            }

            if (!string.IsNullOrEmpty(requireItemId) &&
                (PlayerInventory.Instance == null || !PlayerInventory.Instance.Has(requireItemId)))
            {
                var fallback = (missingItemLines != null && missingItemLines.Length > 0)
                    ? missingItemLines
                    : new[] { "还缺少必要的东西。" };
                DialogueUI.Ensure().Play(speaker, fallback);
                return;
            }

            var dialogue = ResolveSuccessLines();
            DialogueUI.Ensure().Play(dialogue, ApplySuccess);
        }

        List<DialogueLine> ResolveSuccessLines()
        {
            if (sequence != null && sequence.lines != null && sequence.lines.Length > 0)
                return new List<DialogueLine>(sequence.lines);

            var result = new List<DialogueLine>();
            if (lines != null)
            {
                for (int i = 0; i < lines.Length; i++)
                    result.Add(new DialogueLine { speaker = speaker, text = lines[i] });
            }

            if (result.Count == 0)
                result.Add(new DialogueLine { speaker = speaker, text = "……" });

            return result;
        }

        void ApplySuccess()
        {
            if (!string.IsNullOrEmpty(giveItemId) && PlayerInventory.Instance != null)
                PlayerInventory.Instance.Add(giveItemId);

            if (consumeRequiredItem && !string.IsNullOrEmpty(requireItemId) && PlayerInventory.Instance != null)
                PlayerInventory.Instance.Remove(requireItemId);

            used = true;
            var slot = GetComponent<VisualSlot>();
            if (slot != null)
                slot.ApplyUsed();
            onInteractSuccess?.Invoke();

            if (hideAfterSuccess)
                gameObject.SetActive(false);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.85f, 0.3f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}

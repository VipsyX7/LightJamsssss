using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 门锁解开后换成开门图片，并关掉碰撞，让角色可以走过去。
    /// 开门图：Inspector 的 openSprite，或 Resources/Visuals/door_open.png。
    /// </summary>
    public class DoorVisual : MonoBehaviour
    {
        [Tooltip("开门后的图片。留空则读取 door_open。")]
        public Sprite openSprite;

        SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            var interactable = GetComponent<Interactable>();
            if (interactable != null)
                interactable.AddSuccessListener(Open);
        }

        public void Open()
        {
            var sprite = openSprite != null
                ? openSprite
                : VisualLibrary.Get("door_open", () => SpriteFactory.Door(true));

            if (spriteRenderer != null && sprite != null)
                spriteRenderer.sprite = sprite;

            var slot = GetComponent<VisualSlot>();
            if (slot != null)
                slot.ApplyUsed();

            var colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = false;
        }
    }
}

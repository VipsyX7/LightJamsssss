using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 门锁解开后替换开门外观，并关掉碰撞，让角色可以走过去。
    /// </summary>
    public class DoorVisual : MonoBehaviour
    {
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
            if (spriteRenderer != null)
                spriteRenderer.sprite = SpriteFactory.Door(true);

            var colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = false;
        }
    }
}

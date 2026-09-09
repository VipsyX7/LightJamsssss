using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 挂在任意物体上：优先用 Inspector 里拖入的图片，否则按 visualId 去素材库查找。
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class VisualSlot : MonoBehaviour
    {
        [Tooltip("对应 Resources/Visuals 下的文件名，不含扩展名。例如 player、lamp。")]
        public string visualId;

        [Tooltip("直接拖一张 Sprite。有值时优先生效。")]
        public Sprite overrideSprite;

        [Tooltip("交互成功后切换的图片，例如开门、拿走钥匙后的台灯。")]
        public Sprite usedSprite;

        [Tooltip("大于 0 时按这个世界高度等比缩放。")]
        public float fitHeight;

        [Tooltip("换图时是否按图片重算法碰撞。门等需要窄碰撞的物体请关掉。")]
        public bool resizeCollider = true;

        public Vector2 fitWorldSize;
        public bool preserveAspect;

        SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (!string.IsNullOrEmpty(visualId) || overrideSprite != null)
                Apply(false);
        }

        public void Apply(bool used)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                return;

            Sprite sprite = null;
            if (used)
                sprite = usedSprite != null ? usedSprite : VisualLibrary.Find(visualId + "_used");
            if (sprite == null)
                sprite = overrideSprite != null ? overrideSprite : VisualLibrary.Find(visualId);
            if (sprite == null)
                return;

            spriteRenderer.sprite = sprite;
            if (fitHeight > 0.001f)
                SpriteUtil.FitHeight(transform, sprite, fitHeight);
            else if (fitWorldSize.x > 0.001f && fitWorldSize.y > 0.001f)
                SpriteUtil.FitWorldSize(transform, sprite, fitWorldSize, preserveAspect);

            var box = GetComponent<BoxCollider2D>();
            if (resizeCollider && box != null)
                SpriteUtil.FitBoxCollider(box, spriteRenderer);
        }

        public void ApplyUsed()
        {
            Apply(true);
        }
    }
}

using UnityEngine;

namespace LightJam
{
    public static class SpriteUtil
    {
        public static void Apply(
            SpriteRenderer renderer,
            Sprite sprite,
            Vector2 worldSize,
            bool preserveAspect)
        {
            if (renderer == null || sprite == null)
                return;

            renderer.sprite = sprite;
            FitWorldSize(renderer.transform, sprite, worldSize, preserveAspect);
        }

        public static void FitWorldSize(Transform target, Sprite sprite, Vector2 worldSize, bool preserveAspect)
        {
            if (target == null || sprite == null)
                return;
            if (worldSize.x <= 0.001f || worldSize.y <= 0.001f)
            {
                target.localScale = Vector3.one;
                return;
            }

            Vector2 native = sprite.bounds.size;
            if (native.x <= 0.0001f || native.y <= 0.0001f)
                return;

            if (!preserveAspect)
            {
                target.localScale = new Vector3(worldSize.x / native.x, worldSize.y / native.y, 1f);
                return;
            }

            float scale = Mathf.Max(worldSize.x / native.x, worldSize.y / native.y);
            target.localScale = new Vector3(scale, scale, 1f);
        }

        public static void FitHeight(Transform target, Sprite sprite, float worldHeight)
        {
            if (target == null || sprite == null || worldHeight <= 0.001f)
            {
                if (target != null)
                    target.localScale = Vector3.one;
                return;
            }

            float native = sprite.bounds.size.y;
            if (native <= 0.0001f)
                return;
            float scale = worldHeight / native;
            target.localScale = new Vector3(scale, scale, 1f);
        }

        public static void FitBoxCollider(BoxCollider2D box, SpriteRenderer renderer)
        {
            if (box == null || renderer == null || renderer.sprite == null)
                return;

            Bounds bounds = renderer.sprite.bounds;
            box.size = bounds.size;
            box.offset = bounds.center;
        }

        public static void AlignTop(Transform target, Sprite sprite, float worldTopY)
        {
            if (target == null || sprite == null)
                return;

            var position = target.position;
            position.y = worldTopY - sprite.bounds.max.y * target.localScale.y;
            target.position = position;
        }
    }
}

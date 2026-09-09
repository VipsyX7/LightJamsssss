using System;
using System.Collections.Generic;
using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 解析画面素材：Inspector 配置 &gt; Resources/Visuals 同名图片 &gt; 程序占位图。
    /// </summary>
    public static class VisualLibrary
    {
        public const string ResourcesFolder = "Visuals";

        static GameVisuals overrideConfig;
        static readonly Dictionary<string, Sprite> runtimeCache = new Dictionary<string, Sprite>();

        public static GameVisuals Override
        {
            get => overrideConfig;
            set => overrideConfig = value;
        }

        public static void LoadDefaultConfig()
        {
            if (overrideConfig == null)
                overrideConfig = Resources.Load<GameVisuals>("GameVisuals");
        }

        public static Sprite Get(string id, Func<Sprite> fallback)
        {
            var sprite = Find(id);
            if (sprite != null)
                return sprite;
            return fallback != null ? fallback() : null;
        }

        public static Sprite Find(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (overrideConfig != null)
            {
                var configured = overrideConfig.Get(id);
                if (configured != null)
                    return configured;
            }

            var fromResources = Resources.Load<Sprite>(ResourcesFolder + "/" + id);
            if (fromResources != null)
                return fromResources;

            if (runtimeCache.TryGetValue(id, out var cached) && cached != null)
                return cached;

            var texture = Resources.Load<Texture2D>(ResourcesFolder + "/" + id);
            if (texture == null)
                return null;

            var created = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                GuessPivot(id),
                SpriteFactory.PixelsPerUnit);
            created.name = id;
            runtimeCache[id] = created;
            return created;
        }

        public static Vector2 GuessPivot(string id)
        {
            switch (id)
            {
                case "player":
                case "note":
                case "lamp":
                case "lamp_used":
                case "door_closed":
                case "door_open":
                    return new Vector2(0.5f, 0f);
                case "ground":
                    return new Vector2(0.5f, 1f);
                default:
                    return new Vector2(0.5f, 0.5f);
            }
        }
    }
}

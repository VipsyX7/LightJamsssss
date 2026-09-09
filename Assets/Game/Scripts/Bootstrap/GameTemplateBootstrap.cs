using UnityEngine;
using UnityEngine.SceneManagement;

namespace LightJam
{
    /// <summary>
    /// 空场景按下 Play 时自动生成演示关卡。
    /// 画面优先用下方拖入的素材，其次用 Resources/Visuals 同名图片。
    /// </summary>
    public sealed class GameTemplateBootstrap : MonoBehaviour
    {
        [Header("画面素材")]
        [Tooltip("把图片拖到这个资源的槽位里。也可以直接替换 Resources/Visuals 下的同名 PNG。")]
        [SerializeField] GameVisuals visuals;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBuild()
        {
            if (Object.FindFirstObjectByType<PlayerController>() != null)
                return;

            var go = new GameObject("GameTemplateBootstrap");
            go.AddComponent<GameTemplateBootstrap>();
        }

        void Awake()
        {
            if (visuals != null)
                VisualLibrary.Override = visuals;
            else
                VisualLibrary.LoadDefaultConfig();

            BuildDemoLevel();
        }

        public static void BuildDemoLevel()
        {
            if (Object.FindFirstObjectByType<PlayerController>() != null)
                return;

            TuneCamera();
            DialogueUI.Ensure();

            var config = VisualLibrary.Override;
            Vector2 backdropSize = config != null ? config.backdropSize : new Vector2(26f, 10f);
            Vector2 floorSize = config != null ? config.floorSize : new Vector2(26f, 3.2f);
            Vector2 groundSize = config != null ? config.groundSize : new Vector2(24f, 1.1f);
            Vector2 wallSize = config != null ? config.wallSize : new Vector2(0.7f, 8f);
            float playerHeight = config != null ? config.playerHeight : 1.6f;

            CreateWorldSprite(
                "Backdrop",
                "backdrop",
                () => SpriteFactory.Solid(new Color(0.16f, 0.17f, 0.24f, 1f), 32, 32),
                new Vector3(0.5f, 1.4f, 1f),
                backdropSize,
                -20,
                false,
                true);

            CreateWorldSprite(
                "FloorTint",
                "floor",
                () => SpriteFactory.Solid(new Color(0.12f, 0.1f, 0.11f, 1f), 32, 32),
                new Vector3(0.5f, -1.35f, 1f),
                floorSize,
                -10,
                false,
                false);

            CreateWorldSprite(
                "Ground",
                "ground",
                () => SpriteFactory.Rect(new Color(0.36f, 0.29f, 0.25f), new Color(0.22f, 0.18f, 0.16f), 32, 32),
                new Vector3(0.6f, -2.55f, 0f),
                groundSize,
                2,
                true,
                false,
                -2f);

            CreateWorldSprite(
                "LeftWall",
                "wall",
                () => SpriteFactory.Rect(new Color(0.22f, 0.24f, 0.32f), new Color(0.14f, 0.15f, 0.22f), 32, 32),
                new Vector3(-10.2f, 1.2f, 0f),
                wallSize,
                3,
                true,
                false);

            CreateWorldSprite(
                "RightWall",
                "wall",
                () => SpriteFactory.Rect(new Color(0.22f, 0.24f, 0.32f), new Color(0.14f, 0.15f, 0.22f), 32, 32),
                new Vector3(11.4f, 1.2f, 0f),
                wallSize,
                3,
                true,
                false);

            CreateProp("Window", "window", SpriteFactory.Window, new Vector3(-6.4f, 1.35f, 0f), 1.3f, 5, false);
            CreateProp("Painting", "painting", SpriteFactory.Painting, new Vector3(-2.6f, 1.15f, 0f), 1.15f, 5, false);

            var player = CreatePlayer(new Vector3(-7.2f, -2f, 0f), playerHeight);
            FollowCamera(player.transform);

            CreateInteractable(
                "PaintingInspect",
                "painting",
                SpriteFactory.Painting,
                new Vector3(-2.6f, -2f, 0f),
                interactable =>
                {
                    interactable.prompt = "按 E 查看挂画";
                    interactable.speaker = "你";
                    interactable.lines = new[]
                    {
                        "一幅褪色的油画。画里的人背对着窗口，手里握着什么东西。",
                        "画框内侧刻着一行小字：旧铜只认灯下的人。"
                    };
                    interactable.radius = 1.8f;
                },
                visible: false);

            CreateInteractable(
                "DeskNote",
                "note",
                SpriteFactory.Note,
                new Vector3(-0.2f, -2f, 0f),
                interactable =>
                {
                    interactable.prompt = "按 E 阅读纸条";
                    interactable.speaker = "残页";
                    interactable.lines = new[]
                    {
                        "书桌抽屉里压着一张发黄的纸条。",
                        "「灯下有光，光下有钥。门外的路，只认旧铜。」",
                        "看来这间屋子把答案藏在台灯附近。"
                    };
                    interactable.radius = 1.7f;
                });

            CreateInteractable(
                "Lamp",
                "lamp",
                SpriteFactory.Lamp,
                new Vector3(3.1f, -2f, 0f),
                interactable =>
                {
                    interactable.prompt = "按 E 检查台灯";
                    interactable.speaker = "你";
                    interactable.lines = new[]
                    {
                        "灯座底部有一层松动的夹板。",
                        "你摸到一把冰凉的黄铜钥匙。"
                    };
                    interactable.usedLines = new[] { "空空的灯座。钥匙已经在你手里了。" };
                    interactable.giveItemId = "brass_key";
                    interactable.radius = 1.7f;
                },
                usedVisualId: "lamp_used");

            var door = CreateInteractable(
                "LockedDoor",
                "door_closed",
                () => SpriteFactory.Door(false),
                new Vector3(8.2f, -2f, 0f),
                interactable =>
                {
                    interactable.prompt = "按 E 开门";
                    interactable.speaker = "旧门";
                    interactable.lines = new[]
                    {
                        "钥匙转了一圈，锁芯发出清脆的响声。",
                        "门后是一条更暗的走廊。故事，才刚刚开始。"
                    };
                    interactable.missingItemLines = new[]
                    {
                        "门死死锁着。钥匙孔里隐约闪着铜绿。",
                        "没有钥匙的话，只能继续在房间里找线索。"
                    };
                    interactable.requireItemId = "brass_key";
                    interactable.usedLines = new[] { "门已经打开。走廊深处还在等你。" };
                    interactable.radius = 2.6f;
                },
                solid: true,
                usedVisualId: "door_open");

            door.AddComponent<DoorVisual>();

            Debug.Log($"[{SceneManager.GetActiveScene().name}] 演示关卡已生成。替换 Assets/Game/Resources/Visuals 下的 PNG 即可换图。");
        }

        static GameObject CreatePlayer(Vector3 position, float height)
        {
            var go = new GameObject("Player");
            go.tag = "Player";
            go.transform.position = position;

            var renderer = go.AddComponent<SpriteRenderer>();
            var sprite = VisualLibrary.Get("player", SpriteFactory.Player);
            renderer.sprite = sprite;
            renderer.sortingOrder = 20;
            SpriteUtil.FitHeight(go.transform, sprite, height);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3.2f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var box = go.AddComponent<BoxCollider2D>();
            SpriteUtil.FitBoxCollider(box, renderer);

            var slot = go.AddComponent<VisualSlot>();
            slot.visualId = "player";
            slot.overrideSprite = sprite;
            slot.fitHeight = height;

            go.AddComponent<PlayerInventory>();
            go.AddComponent<PlayerController>();
            go.AddComponent<PlayerInteractor>();
            return go;
        }

        static GameObject CreateWorldSprite(
            string name,
            string visualId,
            System.Func<Sprite> fallback,
            Vector3 position,
            Vector2 worldSize,
            int order,
            bool withCollider,
            bool preserveAspect,
            float? alignTopY = null)
        {
            var go = new GameObject(name);
            go.transform.position = position;

            var renderer = go.AddComponent<SpriteRenderer>();
            var sprite = VisualLibrary.Get(visualId, fallback);
            renderer.sortingOrder = order;
            SpriteUtil.Apply(renderer, sprite, worldSize, preserveAspect);
            if (alignTopY.HasValue)
                SpriteUtil.AlignTop(go.transform, sprite, alignTopY.Value);

            var slot = go.AddComponent<VisualSlot>();
            slot.visualId = visualId;
            slot.overrideSprite = sprite;
            slot.fitWorldSize = worldSize;
            slot.preserveAspect = preserveAspect;

            if (withCollider)
            {
                var box = go.AddComponent<BoxCollider2D>();
                SpriteUtil.FitBoxCollider(box, renderer);
            }

            return go;
        }

        static GameObject CreateProp(
            string name,
            string visualId,
            System.Func<Sprite> fallback,
            Vector3 position,
            float height,
            int order,
            bool withCollider)
        {
            var go = new GameObject(name);
            go.transform.position = position;

            var renderer = go.AddComponent<SpriteRenderer>();
            var sprite = VisualLibrary.Get(visualId, fallback);
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            SpriteUtil.FitHeight(go.transform, sprite, height);

            var slot = go.AddComponent<VisualSlot>();
            slot.visualId = visualId;
            slot.overrideSprite = sprite;
            slot.fitHeight = height;

            if (withCollider)
            {
                var box = go.AddComponent<BoxCollider2D>();
                SpriteUtil.FitBoxCollider(box, renderer);
            }

            return go;
        }

        static GameObject CreateInteractable(
            string name,
            string visualId,
            System.Func<Sprite> fallback,
            Vector3 position,
            System.Action<Interactable> setup,
            bool visible = true,
            bool solid = false,
            string usedVisualId = null)
        {
            var go = new GameObject(name);
            go.transform.position = position;

            var renderer = go.AddComponent<SpriteRenderer>();
            var sprite = VisualLibrary.Get(visualId, fallback);
            renderer.sprite = sprite;
            renderer.sortingOrder = 8;
            renderer.enabled = visible;
            if (visible && sprite != null && solid)
                SpriteUtil.FitHeight(go.transform, sprite, 2.8f);

            var box = go.AddComponent<BoxCollider2D>();
            if (visible && sprite != null)
            {
                SpriteUtil.FitBoxCollider(box, renderer);
            }
            else
            {
                box.size = new Vector2(1.4f, 0.4f);
                box.offset = new Vector2(0f, 0.2f);
            }

            if (solid)
            {
                box.isTrigger = false;
                float scaleX = Mathf.Max(0.001f, Mathf.Abs(go.transform.localScale.x));
                box.size = new Vector2(0.45f / scaleX, box.size.y);
            }
            else
            {
                box.isTrigger = true;
            }

            var slot = go.AddComponent<VisualSlot>();
            slot.visualId = visualId;
            slot.overrideSprite = sprite;
            slot.resizeCollider = !solid;
            if (!string.IsNullOrEmpty(usedVisualId))
                slot.usedSprite = VisualLibrary.Find(usedVisualId);

            var interactable = go.AddComponent<Interactable>();
            setup?.Invoke(interactable);
            return go;
        }

        static void TuneCamera()
        {
            var cam = Camera.main;
            if (cam == null)
                return;

            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.backgroundColor = new Color(0.09f, 0.1f, 0.14f, 1f);
            cam.clearFlags = CameraClearFlags.SolidColor;
        }

        static void FollowCamera(Transform target)
        {
            var cam = Camera.main;
            if (cam == null)
                return;

            var follow = cam.GetComponent<CameraFollow>();
            if (follow == null)
                follow = cam.gameObject.AddComponent<CameraFollow>();
            follow.SetTarget(target);
        }
    }
}

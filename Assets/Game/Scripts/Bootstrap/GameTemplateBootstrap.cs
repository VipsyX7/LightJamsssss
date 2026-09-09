using UnityEngine;
using UnityEngine.SceneManagement;

namespace LightJam
{
    /// <summary>
    /// 空场景按下 Play 时自动生成演示关卡。
    /// 若场景里已经放了 PlayerController，则不会重复生成。
    /// </summary>
    public sealed class GameTemplateBootstrap : MonoBehaviour
    {
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
            BuildDemoLevel();
        }

        public static void BuildDemoLevel()
        {
            if (Object.FindFirstObjectByType<PlayerController>() != null)
                return;

            TuneCamera();
            DialogueUI.Ensure();

            CreateBackdrop();
            CreateGround();
            CreateWall(-10.2f);
            CreateWall(11.4f);

            CreateDecor("Window", SpriteFactory.Window(), new Vector3(-6.4f, 1.35f, 0f), 1.4f, 5);
            CreateDecor("Painting", SpriteFactory.Painting(), new Vector3(-2.6f, 1.15f, 0f), 1.25f, 5);

            var player = CreatePlayer(new Vector3(-7.2f, -2f, 0f));
            FollowCamera(player.transform);

            CreateInteractable(
                "PaintingInspect",
                SpriteFactory.Painting(),
                new Vector3(-2.6f, -2f, 0f),
                new Vector2(1.4f, 0.4f),
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
                SpriteFactory.Note(),
                new Vector3(-0.2f, -2f, 0f),
                new Vector2(0.8f, 1.1f),
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
                SpriteFactory.Lamp(),
                new Vector3(3.1f, -2f, 0f),
                new Vector2(0.9f, 1.7f),
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
                });

            var door = CreateInteractable(
                "LockedDoor",
                SpriteFactory.Door(false),
                new Vector3(8.2f, -2f, 0f),
                new Vector2(1.5f, 3f),
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
                    interactable.radius = 1.9f;
                },
                solid: true);

            door.AddComponent<DoorVisual>();

            Debug.Log($"[{SceneManager.GetActiveScene().name}] LightJam 演示关卡已生成：A/D 移动，靠近物体后按 E 对话。");
        }

        static GameObject CreatePlayer(Vector3 position)
        {
            var go = new GameObject("Player");
            go.tag = "Player";
            go.transform.position = position;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Player();
            renderer.sortingOrder = 20;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3.2f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.42f, 0.72f);
            box.offset = new Vector2(0f, 0.36f);

            go.AddComponent<PlayerInventory>();
            go.AddComponent<PlayerController>();
            go.AddComponent<PlayerInteractor>();
            return go;
        }

        static void CreateGround()
        {
            CreateSolidBlock(
                "Ground",
                new Vector3(0.6f, -2.55f, 0f),
                new Vector2(24f, 1.1f),
                new Color(0.36f, 0.29f, 0.25f),
                new Color(0.22f, 0.18f, 0.16f),
                2);
        }

        static void CreateWall(float x)
        {
            CreateSolidBlock(
                x < 0f ? "LeftWall" : "RightWall",
                new Vector3(x, 1.2f, 0f),
                new Vector2(0.7f, 8f),
                new Color(0.22f, 0.24f, 0.32f),
                new Color(0.14f, 0.15f, 0.22f),
                3);
        }

        static void CreateBackdrop()
        {
            CreateScaledSprite(
                "Backdrop",
                SpriteFactory.Solid(new Color(0.16f, 0.17f, 0.24f, 1f), 32, 32),
                new Vector3(0.5f, 1.4f, 1f),
                new Vector2(26f, 10f),
                -20,
                false);

            CreateScaledSprite(
                "FloorTint",
                SpriteFactory.Solid(new Color(0.12f, 0.1f, 0.11f, 1f), 32, 32),
                new Vector3(0.5f, -1.35f, 1f),
                new Vector2(26f, 3.2f),
                -10,
                false);
        }

        static void CreateSolidBlock(string name, Vector3 position, Vector2 size, Color fill, Color border, int order)
        {
            var go = CreateScaledSprite(name, SpriteFactory.Rect(fill, border, 32, 32), position, size, order, true);
            var box = go.GetComponent<BoxCollider2D>();
            box.size = Vector2.one;
        }

        static GameObject CreateDecor(string name, Sprite sprite, Vector3 position, float scale, int order)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            go.transform.localScale = Vector3.one * scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            return go;
        }

        static GameObject CreateScaledSprite(
            string name,
            Sprite sprite,
            Vector3 position,
            Vector2 worldSize,
            int order,
            bool withCollider)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            go.transform.localScale = new Vector3(worldSize.x, worldSize.y, 1f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;

            if (withCollider)
                go.AddComponent<BoxCollider2D>();

            return go;
        }

        static GameObject CreateInteractable(
            string name,
            Sprite sprite,
            Vector3 position,
            Vector2 colliderSize,
            System.Action<Interactable> setup,
            bool visible = true,
            bool solid = false)
        {
            var go = new GameObject(name);
            go.transform.position = position;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 8;
            renderer.enabled = visible;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = colliderSize;
            box.offset = new Vector2(0f, colliderSize.y * 0.5f);
            box.isTrigger = !solid;

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

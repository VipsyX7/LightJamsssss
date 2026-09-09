using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 画面素材表。把图片拖进对应槽位即可换皮；
    /// 也可以直接替换 <c>Assets/Game/Resources/Visuals</c> 下的同名 PNG。
    /// </summary>
    [CreateAssetMenu(menuName = "LightJam/画面素材", fileName = "GameVisuals")]
    public class GameVisuals : ScriptableObject
    {
        [Header("场景")]
        [Tooltip("整张房间背景。")]
        public Sprite backdrop;
        [Tooltip("地面装饰层（可选）。")]
        public Sprite floor;
        [Tooltip("角色站立的地面。")]
        public Sprite ground;
        [Tooltip("左右挡墙。")]
        public Sprite wall;

        [Header("角色")]
        public Sprite player;

        [Header("道具")]
        public Sprite window;
        public Sprite painting;
        public Sprite note;
        public Sprite lamp;
        public Sprite lampUsed;
        public Sprite doorClosed;
        public Sprite doorOpen;

        [Header("尺寸（0 表示用图片原始大小）")]
        [Tooltip("角色在世界里的目标身高。")]
        public float playerHeight = 1.6f;
        public Vector2 backdropSize = new Vector2(26f, 10f);
        public Vector2 floorSize = new Vector2(26f, 3.2f);
        public Vector2 groundSize = new Vector2(24f, 1.1f);
        public Vector2 wallSize = new Vector2(0.7f, 8f);

        public Sprite Get(string id)
        {
            switch (id)
            {
                case "backdrop": return backdrop;
                case "floor": return floor;
                case "ground": return ground;
                case "wall": return wall;
                case "player": return player;
                case "window": return window;
                case "painting": return painting;
                case "note": return note;
                case "lamp": return lamp;
                case "lamp_used": return lampUsed;
                case "door_closed": return doorClosed;
                case "door_open": return doorOpen;
                default: return null;
            }
        }
    }
}

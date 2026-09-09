using UnityEngine;

namespace LightJam
{
    [System.Serializable]
    public class DialogueLine
    {
        [Tooltip("说话人名称，可留空。")]
        public string speaker;

        [TextArea(2, 6)]
        public string text;
    }

    /// <summary>
    /// 可在 Project 窗口右键创建的对话资源，方便策划填剧情而不改代码。
    /// </summary>
    [CreateAssetMenu(menuName = "LightJam/对话序列", fileName = "NewDialogue")]
    public class DialogueSequence : ScriptableObject
    {
        public DialogueLine[] lines;
    }
}

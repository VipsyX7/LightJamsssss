using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LightJam
{
    /// <summary>
    /// 底部对话框 + 交互提示。支持打字机效果，按 E 加速/翻页。
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance { get; private set; }

        [SerializeField] float charsPerSecond = 42f;

        Canvas canvas;
        GameObject panel;
        GameObject promptRoot;
        Text speakerText;
        Text bodyText;
        Text continueText;
        Text promptText;
        Text helpText;

        readonly List<DialogueLine> queue = new List<DialogueLine>();
        int index;
        string fullText = "";
        float typed;
        bool lineComplete;
        Action onClosed;

        public bool IsOpen => GameState.IsDialogueOpen;

        public static DialogueUI Ensure()
        {
            if (Instance != null)
                return Instance;

            var go = new GameObject("DialogueUI");
            return go.AddComponent<DialogueUI>();
        }

        void Awake()
        {
            Instance = this;
            BuildUi();
            HideDialogue();
            ShowPrompt(false, string.Empty);
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            GameState.IsDialogueOpen = false;
        }

        void Update()
        {
            if (!GameState.IsDialogueOpen)
                return;

            if (!lineComplete)
            {
                typed += Time.deltaTime * charsPerSecond;
                int count = Mathf.Clamp(Mathf.FloorToInt(typed), 0, fullText.Length);
                bodyText.text = fullText.Substring(0, count);
                if (count >= fullText.Length)
                    CompleteLine();
            }

            if (!GameInput.InteractPressed)
                return;

            GameInput.ConsumeInteract();

            if (!lineComplete)
            {
                CompleteLine();
                return;
            }

            ShowNext();
        }

        public void ShowPrompt(bool visible, string message)
        {
            if (GameState.IsDialogueOpen)
                visible = false;

            promptRoot.SetActive(visible);
            promptText.text = message;
        }

        public void Play(IList<DialogueLine> lines, Action closed = null)
        {
            if (lines == null || lines.Count == 0)
            {
                closed?.Invoke();
                return;
            }

            queue.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] != null && !string.IsNullOrWhiteSpace(lines[i].text))
                    queue.Add(lines[i]);
            }

            if (queue.Count == 0)
            {
                closed?.Invoke();
                return;
            }

            onClosed = closed;
            index = -1;
            GameState.IsDialogueOpen = true;
            panel.SetActive(true);
            ShowPrompt(false, string.Empty);
            ShowNext();
        }

        public void Play(DialogueSequence sequence, Action closed = null)
        {
            Play(sequence != null ? sequence.lines : null, closed);
        }

        public void Play(string speaker, string[] texts, Action closed = null)
        {
            var lines = new List<DialogueLine>();
            if (texts != null)
            {
                for (int i = 0; i < texts.Length; i++)
                    lines.Add(new DialogueLine { speaker = speaker, text = texts[i] });
            }

            Play(lines, closed);
        }

        void ShowNext()
        {
            index++;
            if (index >= queue.Count)
            {
                HideDialogue();
                var done = onClosed;
                onClosed = null;
                done?.Invoke();
                return;
            }

            var line = queue[index];
            speakerText.text = string.IsNullOrEmpty(line.speaker) ? " " : line.speaker;
            fullText = line.text.Trim();
            typed = 0f;
            lineComplete = false;
            bodyText.text = "";
            continueText.text = "按 E 跳过";
        }

        void CompleteLine()
        {
            typed = fullText.Length;
            bodyText.text = fullText;
            lineComplete = true;
            continueText.text = index < queue.Count - 1 ? "按 E 继续" : "按 E 关闭";
        }

        void HideDialogue()
        {
            panel.SetActive(false);
            GameState.IsDialogueOpen = false;
            bodyText.text = "";
            speakerText.text = "";
        }

        void BuildUi()
        {
            var font = LoadUiFont();

            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            gameObject.AddComponent<GraphicRaycaster>();

            helpText = CreateText(transform, "Help", font, 26, new Color(0.86f, 0.84f, 0.78f, 0.7f), TextAnchor.UpperCenter);
            Stretch(helpText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(40f, -70f), new Vector2(-40f, -16f));
            helpText.text = "A / D 移动    E 交互";

            promptRoot = new GameObject("Prompt", typeof(RectTransform));
            promptRoot.transform.SetParent(transform, false);
            var promptBg = promptRoot.AddComponent<Image>();
            promptBg.color = new Color(0.08f, 0.08f, 0.1f, 0.72f);
            var promptRt = promptRoot.GetComponent<RectTransform>();
            promptRt.anchorMin = new Vector2(0.5f, 0.22f);
            promptRt.anchorMax = new Vector2(0.5f, 0.22f);
            promptRt.sizeDelta = new Vector2(520f, 56f);
            promptText = CreateText(promptRoot.transform, "PromptText", font, 30, new Color(0.95f, 0.9f, 0.72f), TextAnchor.MiddleCenter);
            Stretch(promptText.rectTransform, Vector2.zero, Vector2.one, new Vector2(16f, 4f), new Vector2(-16f, -4f));

            panel = new GameObject("DialoguePanel", typeof(RectTransform));
            panel.transform.SetParent(transform, false);
            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.07f, 0.07f, 0.09f, 0.92f);
            Stretch(panel.GetComponent<RectTransform>(), new Vector2(0.08f, 0f), new Vector2(0.92f, 0f), new Vector2(0f, 36f), new Vector2(0f, 280f));

            var accent = new GameObject("Accent", typeof(RectTransform));
            accent.transform.SetParent(panel.transform, false);
            accent.AddComponent<Image>().color = new Color(0.83f, 0.68f, 0.36f, 1f);
            Stretch(accent.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, new Vector2(0f, 4f));

            speakerText = CreateText(panel.transform, "Speaker", font, 32, new Color(0.93f, 0.78f, 0.42f), TextAnchor.UpperLeft);
            Stretch(speakerText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(36f, -70f), new Vector2(-36f, -18f));

            bodyText = CreateText(panel.transform, "Body", font, 34, new Color(0.93f, 0.91f, 0.86f), TextAnchor.UpperLeft);
            bodyText.horizontalOverflow = HorizontalWrapMode.Wrap;
            bodyText.verticalOverflow = VerticalWrapMode.Overflow;
            Stretch(bodyText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(36f, 48f), new Vector2(-36f, -78f));

            continueText = CreateText(panel.transform, "Continue", font, 24, new Color(0.75f, 0.72f, 0.64f), TextAnchor.LowerRight);
            Stretch(continueText.rectTransform, Vector2.zero, Vector2.one, new Vector2(36f, 16f), new Vector2(-36f, -12f));
        }

        static Text CreateText(Transform parent, string name, Font font, int size, Color color, TextAnchor align)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.color = color;
            text.alignment = align;
            text.raycastTarget = false;
            text.supportRichText = true;
            return text;
        }

        static void Stretch(RectTransform rt, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
        }

        static Font LoadUiFont()
        {
            return Font.CreateDynamicFontFromOSFont(new[]
            {
                "Microsoft YaHei UI",
                "Microsoft YaHei",
                "SimHei",
                "PingFang SC",
                "Noto Sans CJK SC",
                "Arial Unicode MS",
                "Arial"
            }, 24);
        }
    }
}

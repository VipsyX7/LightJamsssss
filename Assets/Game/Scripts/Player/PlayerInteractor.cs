using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 检测最近的可交互物，显示提示，并在按下 E 时打开对话。
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        Interactable current;
        bool waitForInteractRelease;

        void Update()
        {
            if (GameState.IsDialogueOpen)
            {
                waitForInteractRelease = true;
                DialogueUI.Ensure().ShowPrompt(false, string.Empty);
                current = null;
                return;
            }

            if (waitForInteractRelease)
            {
                if (GameInput.InteractHeld)
                    return;
                waitForInteractRelease = false;
            }

            current = Interactable.FindNearest(transform.position);
            if (current == null)
            {
                DialogueUI.Ensure().ShowPrompt(false, string.Empty);
                return;
            }

            DialogueUI.Ensure().ShowPrompt(true, current.prompt);

            if (GameInput.InteractPressed)
            {
                GameInput.ConsumeInteract();
                current.Interact();
            }
        }
    }
}

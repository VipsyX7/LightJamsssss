using UnityEngine;
using UnityEngine.InputSystem;

namespace LightJam
{
    /// <summary>
    /// 读取键盘：A/D（或方向键）移动，E 交互。
    /// 项目启用的是新 Input System，因此不使用旧版 Input.GetKey。
    /// </summary>
    public static class GameInput
    {
        static int consumedInteractFrame = -1;
        public static float Horizontal
        {
            get
            {
                var keyboard = Keyboard.current;
                if (keyboard == null)
                    return 0f;

                float value = 0f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    value -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    value += 1f;
                return value;
            }
        }

        public static bool InteractPressed
        {
            get
            {
                if (Time.frameCount == consumedInteractFrame)
                    return false;

                var keyboard = Keyboard.current;
                return keyboard != null && keyboard.eKey.wasPressedThisFrame;
            }
        }

        public static bool InteractHeld
        {
            get
            {
                var keyboard = Keyboard.current;
                return keyboard != null && keyboard.eKey.isPressed;
            }
        }

        public static void ConsumeInteract()
        {
            consumedInteractFrame = Time.frameCount;
        }
    }
}

namespace LightJam
{
    /// <summary>
    /// 全局游玩状态。对话框打开时锁定移动，避免剧情被走动打断。
    /// </summary>
    public static class GameState
    {
        public static bool IsDialogueOpen { get; set; }

        public static bool CanMove => !IsDialogueOpen;
    }
}

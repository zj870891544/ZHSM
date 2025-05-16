namespace ZHSM
{
    /// <summary>
    /// 服务端游戏中界面
    /// </summary>
    public class ServerGamePlayForm : UGuiForm
    {
        /// <summary>
        /// 退出到准备流程
        /// </summary>
        public void ExitGame()
        {
            GameEntry.UI.OpenDialog(new DialogParams
            {
                Mode = 2,
                Title = "退出游戏",
                Message = "结束游戏并返回？",
                ConfirmText = "确认",
                OnClickConfirm = o =>
                {
                    // GameEntry.Network.ReturnToRoom();
                },
                CancelText = "取消"
            });
        }
    }
}
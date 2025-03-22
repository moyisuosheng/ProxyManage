using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ProxyManage
{
    /// <summary>
    /// 消息工具
    /// </summary>
    class MessageUtil
    {
        /// <summary>
        /// 显示成功消息
        /// </summary>
        /// <param name="message"></param>
        public static async void ShowSuccessMessage(string message)
        {
            IToast toast = Toast.Make("✅ " + message, ToastDuration.Short);
            await toast.Show();
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="message"></param>
        public static async void ShowErrorMessage(string message)
        {
            IToast toast = Toast.Make("❌ " + message, ToastDuration.Short);
            await toast.Show();
        }
    }
}

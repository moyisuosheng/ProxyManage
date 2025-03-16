using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProxyManage
{
    public partial class ConfigPage : ContentPage
    {
        /// <summary>
        /// 索引，当index < 0 时表示新建配置，否则表示编辑配置
        /// </summary>
        private readonly int index;

        /// <summary>
        /// 配置
        /// </summary>
        private readonly Config config;

        /// <summary>
        /// 是否已保存
        /// </summary>
        private bool IsSaved;

        public ConfigPage(int index, Config config)
        {
            InitializeComponent();
            this.index = index;
            this.config = config;
            BindingContext = config;
        }

        /// <summary>
        /// 当保存按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            // 校验用户输入
            var (IsValid, ErrorMessage) = ValidateConfig(config);
            if (!IsValid)
            {
                // 显示错误消息
                MessageUtil.ShowErrorMessage(ErrorMessage);
                return;
            }

            IsSaved = true;
            Debug.WriteLine("保存");
            try
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 当取消按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("取消");

            IsSaved = false;
            try
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"导航失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取索引，当index < 0 时表示新建配置，否则表示编辑配置
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            return index;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public Config GetConfig()
        {
            return config;
        }

        /// <summary>
        /// 获取是否已保存, 用于判断是否需要保存配置 true: 已保存 false: 未保存
        /// </summary>
        /// <returns></returns>
        public bool GetIsSaved()
        {
            return IsSaved;
        }

        /// <summary>
        /// 当 Entry 文本改变时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry) return;

            // 使用正则表达式验证输入是否为有效的网址和端口
            if (entry == ProxyServerEntry && !IsValidUrl(entry.Text))
            {
                // 显示错误消息或清除无效输入
                entry.TextColor = Colors.Red;
                SaveButton.IsEnabled = false;
            }
            else
            {
                entry.TextColor = Colors.Black;
            }

            // 校验所有输入是否有效
            var (IsValid, ErrorMessage) = ValidateConfig(config);
            if (IsValid)
            {
                SaveButton.IsEnabled = true;
            }
            else
            {
                SaveButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// 校验配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private (bool IsValid, string ErrorMessage) ValidateConfig(Config config)
        {
            if (string.IsNullOrWhiteSpace(config.Name))
            {
                return (false, "配置名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(config.ProxyServer))
            {
                return (false, "代理服务器地址不能为空");
            }
            // 其他校验逻辑...
            if (!IsValidUrl(config.ProxyServer))
            {
                return (false, "代理服务器地址无效");
            }
            if (!ValidateBypassList(config.BypassList ?? ""))
            {
                return (false, "绕过列表无效");
            }
            return (true, string.Empty);
        }

        /// <summary>
        /// 当 BypassListEditor 文本改变时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBypassListEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            var editor = sender as Editor;
            if (editor == null) return;

            string text = editor.Text;
            if (ValidateBypassList(text))
            {
                editor.TextColor = Colors.Black;
            }
            else
            {
                editor.TextColor = Colors.Red;
            }

            // 校验所有输入是否有效
            var (IsValid, ErrorMessage) = ValidateConfig(config);
            if (IsValid)
            {
                SaveButton.IsEnabled = true;
            }
            else
            {
                SaveButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// 验证是否为正确路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsValidUrl(string url)
        {
            if (url == null)
            {
                return false;
            }
            //Regex regex = new Regex(@"^[a-zA-Z0-9.:=/?]*$");
            //if (!regex.IsMatch(url))
            //{
            //    // 移除无效字符
            //    return false;
            //}

            // 尝试将字符串解析为 Uri 对象
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult))
            {
                // 进一步验证协议（例如只允许 http/https/ftp）
                return uriResult.Scheme == Uri.UriSchemeHttp
                    || uriResult.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }

        /// <summary>
        /// 验证是否为正确路径
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool ValidateBypassList(string text)
        {
            if (null == text)
            {
                return true;
            }
            List<string> list = text.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (list.Count == 0)
            {
                return true;
            }
            Regex regex = new Regex(@"^[a-zA-Z0-9.:=/?*%]*$");
            // 验证每个元素是否为有效的 URL
            return list.All(regex.IsMatch);
        }
    }
}
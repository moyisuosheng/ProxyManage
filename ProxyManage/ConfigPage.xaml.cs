using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProxyManage
{
    public partial class ConfigPage : ContentPage
    {
        /// <summary>
        /// ��������index < 0 ʱ��ʾ�½����ã������ʾ�༭����
        /// </summary>
        private readonly int index;

        /// <summary>
        /// ����
        /// </summary>
        private readonly Config config;

        /// <summary>
        /// �Ƿ��ѱ���
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
        /// �����水ť�����ʱ�Ļص�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            // У���û�����
            var (IsValid, ErrorMessage) = ValidateConfig(config);
            if (!IsValid)
            {
                // ��ʾ������Ϣ
                MessageUtil.ShowErrorMessage(ErrorMessage);
                return;
            }

            IsSaved = true;
            Debug.WriteLine("����");
            try
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"����ʧ��: {ex.Message}");
            }
        }

        /// <summary>
        /// ��ȡ����ť�����ʱ�Ļص�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("ȡ��");

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
                Debug.WriteLine($"����ʧ��: {ex.Message}");
            }
        }

        /// <summary>
        /// ��ȡ��������index < 0 ʱ��ʾ�½����ã������ʾ�༭����
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            return index;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        public Config GetConfig()
        {
            return config;
        }

        /// <summary>
        /// ��ȡ�Ƿ��ѱ���, �����ж��Ƿ���Ҫ�������� true: �ѱ��� false: δ����
        /// </summary>
        /// <returns></returns>
        public bool GetIsSaved()
        {
            return IsSaved;
        }

        /// <summary>
        /// �� Entry �ı��ı�ʱ�Ļص�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry) return;

            // ʹ��������ʽ��֤�����Ƿ�Ϊ��Ч����ַ�Ͷ˿�
            if (entry == ProxyServerEntry && !IsValidUrl(entry.Text))
            {
                // ��ʾ������Ϣ�������Ч����
                entry.TextColor = Colors.Red;
                SaveButton.IsEnabled = false;
            }
            else
            {
                entry.TextColor = Colors.Black;
            }

            // У�����������Ƿ���Ч
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
        /// У������
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private (bool IsValid, string ErrorMessage) ValidateConfig(Config config)
        {
            if (string.IsNullOrWhiteSpace(config.Name))
            {
                return (false, "�������Ʋ���Ϊ��");
            }
            if (string.IsNullOrWhiteSpace(config.ProxyServer))
            {
                return (false, "�����������ַ����Ϊ��");
            }
            // ����У���߼�...
            if (!IsValidUrl(config.ProxyServer))
            {
                return (false, "�����������ַ��Ч");
            }
            if (!ValidateBypassList(config.BypassList ?? ""))
            {
                return (false, "�ƹ��б���Ч");
            }
            return (true, string.Empty);
        }

        /// <summary>
        /// �� BypassListEditor �ı��ı�ʱ�Ļص�
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

            // У�����������Ƿ���Ч
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
        /// ��֤�Ƿ�Ϊ��ȷ·��
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
            //    // �Ƴ���Ч�ַ�
            //    return false;
            //}

            // ���Խ��ַ�������Ϊ Uri ����
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult))
            {
                // ��һ����֤Э�飨����ֻ���� http/https/ftp��
                return uriResult.Scheme == Uri.UriSchemeHttp
                    || uriResult.Scheme == Uri.UriSchemeHttps;
            }
            return false;
        }

        /// <summary>
        /// ��֤�Ƿ�Ϊ��ȷ·��
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
            // ��֤ÿ��Ԫ���Ƿ�Ϊ��Ч�� URL
            return list.All(regex.IsMatch);
        }
    }
}
namespace ProxyManage
{
    public partial class MainPage : ContentPage
    {

        private readonly IProxyUtil proxyUtil;

        private readonly IConfigUtil configUtil;

        private readonly IFolderOpener folderOpener;

        public MainPage()
        {
            InitializeComponent();

            proxyUtil = DependencyService.Get<IProxyUtil>();
            if (null == proxyUtil)
            {
                throw new Exception("IProxyUtil 未注册");
            }

            configUtil = DependencyService.Get<IConfigUtil>();
            if (null == configUtil)
            {
                throw new Exception("IConfigUtil 未注册");
            }

            folderOpener = DependencyService.Get<IFolderOpener>();
        }

        private void OnDisableProxyClicked(object sender, EventArgs e)
        {
            // 关闭代理
            if (proxyUtil.DisableSystemProxy())
            {
                DisplayAlert("成功", $"代理已禁用。", "确定");
            }
            else
            {
                DisplayAlert("失败", $"代理未禁用。", "确定");
            }
        }

        private void OnEnableProxyForItemClicked(object sender, EventArgs e)
        {

            var button = sender as Button;
            var config = button?.CommandParameter as Config;

            if (null != config && null != config.ProxyServer)
            {
                if (proxyUtil.SetSystemProxy(config.ProxyServer, config.BypassList ?? ""))
                {
                    DisplayAlert("成功", $"代理已启用: {config.Name}", "确定");
                }
                else
                {
                    DisplayAlert("失败", $"代理未启用: {config.Name}", "确定");
                }

            }
        }
        private void OnSaveConfigClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as MainViewModel;
            if (viewModel != null)
            {
                List<Config> configs = viewModel.getConfiguration();
                configUtil.SaveConfiguration(configs ?? []);
                DisplayAlert("成功", "配置已保存", "确定");
            }
        }

        private void OnOpenFolderClicked(object sender, EventArgs e)
        {
            //var folderPath = Path.Combine(FileSystem.AppDataDirectory, "ProxyManage");

            string configFolderPath = configUtil.getFolder();
            if (folderOpener == null)
            {
                DisplayAlert("错误", "无法打开文件夹。", "确定");
                return;
            }
            folderOpener.OpenFolder(configFolderPath);
        }
    }

}

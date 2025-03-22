using System.Diagnostics;

namespace ProxyManage
{
    /// <summary>
    /// 主页面
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// 代理工具
        /// </summary>
        private readonly IProxyUtil proxyUtil;

        /// <summary>
        /// 配置工具
        /// </summary>
        private readonly IConfigUtil configUtil;

        /// <summary>
        /// 文件夹打开器
        /// </summary>
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

        /// <summary>
        /// 当页面显示时的回调
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // 订阅事件，当配置页面被弹出并且用户点击了保存按钮时，更新主页面的配置列表
            if (Application.Current?.MainPage is NavigationPage navigationPage)
            {
                navigationPage.Popped += OnPagePopped;
            }
            else
            {
                throw new Exception("NavigationPage 未设置为 MainPage");
            }
        }

        /// <summary>
        /// 当页面消失时的回调
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // 取消订阅事件
            if (Application.Current?.MainPage is NavigationPage navigationPage)
            {
                navigationPage.Popped -= OnPagePopped;
            }
        }

        /// <summary>
        /// 当页面被弹出时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnPagePopped(object? sender, NavigationEventArgs e)
        {
            Debug.WriteLine("回调");

            if (e.Page is ConfigPage configPage && configPage.GetIsSaved())
            {
                // 用户点击了保存按钮，更新主页面的配置列表
                if (BindingContext is MainViewModel viewModel)
                {
                    // 获取配置页面的配置的索引
                    int index = configPage.GetIndex();

                    if (index < 0)
                    {
                        // 新增配置
                        viewModel.AddNewConfig(configPage.GetConfig());
                        List<Config> configs = viewModel.GetConfiguration();
                        configUtil.SaveConfiguration(configs ?? []);
                        MessageUtil.ShowSuccessMessage("配置已新增并保存！");
                    }
                    else
                    {
                        // 更新配置
                        viewModel.UpdateConfigByIndex(configPage.GetIndex(), configPage.GetConfig());

                        List<Config> configs = viewModel.GetConfiguration();
                        configUtil.SaveConfiguration(configs ?? []);
                        MessageUtil.ShowSuccessMessage("配置已保存！");
                    }
                }
            }
        }

        /// <summary>
        /// 当禁用代理按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEnableProxyClicked(object sender, EventArgs e)
        {
            // 关闭代理
            if (proxyUtil.EnableSystemProxy())
            {
                MessageUtil.ShowSuccessMessage("代理已启用！");
            }
            else
            {
                MessageUtil.ShowErrorMessage("代理未启用！");
            }
        }

        /// <summary>
        /// 当禁用代理按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisableProxyClicked(object sender, EventArgs e)
        {
            // 关闭代理
            if (proxyUtil.DisableSystemProxy())
            {
                MessageUtil.ShowSuccessMessage("代理已禁用！");
            }
            else
            {
                MessageUtil.ShowErrorMessage("代理未禁用！");
            }
        }

        /// <summary>
        /// 当启用代理按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEnableProxyForItemClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;

            if (button?.CommandParameter is Config config && null != config.ProxyServer)
            {
                if (proxyUtil.SetSystemProxy(config))
                {
                    MessageUtil.ShowSuccessMessage($"代理已启用: {config.Name}");
                }
                else
                {
                    MessageUtil.ShowErrorMessage($"代理未启用: {config.Name}");
                }

            }
        }

        /// <summary>
        /// 当打开文件夹按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenFolderClicked(object sender, EventArgs e)
        {
            //var folderPath = Path.Combine(FileSystem.AppDataDirectory, "ProxyManage");

            string configFolderPath = configUtil.GetFolderPath();
            if (folderOpener == null)
            {
                MessageUtil.ShowErrorMessage("无法打开文件夹。");
                return;
            }
            folderOpener.OpenFolder(configFolderPath);
        }

        /// <summary>
        /// 当编辑配置按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private async void OnEditConfigClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;

            if (button?.CommandParameter is Config config)
            {
                if (BindingContext is MainViewModel viewModel)
                {
                    int index = viewModel.GetIndexOfConfig(config);
                    if (index < 0)
                    {
                        throw new Exception("Invalid index");
                    }
                    ConfigPage configPage = new(index, config);
                    await Navigation.PushAsync(configPage);
                }
            }
        }

        /// <summary>
        /// 当删除配置按钮被点击时的回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDeleteConfigClicked(object sender, EventArgs e)
        {
            Button? button = sender as Button;

            if (button?.CommandParameter is Config config)
            {
                if (BindingContext is MainViewModel viewModel)
                {
                    bool answer = await DisplayAlert("确认删除", $"确认要删除 {config.Name} 配置吗？", "删除", "取消");
                    if (answer)
                    {
                        // 执行删除操作
                        if (viewModel.RemoveConfig(config))
                        {
                            List<Config> configs = viewModel.GetConfiguration();
                            configUtil.SaveConfiguration(configs ?? []);
                            MessageUtil.ShowSuccessMessage($"配置已删除: {config.Name}");
                        }
                        else
                        {
                            MessageUtil.ShowErrorMessage($"配置未删除: {config.Name}");
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 当添加新配置按钮被点击时的回调
        /// </summary>
        public async void OnAddNewConfigClicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel viewModel)
            {
                Config newConfig = new()
                {
                    Name = "",
                    ProxyServer = "",
                    BypassList = "",
                    Local = false
                };

                ConfigPage configPage = new(-1, newConfig);
                await Navigation.PushAsync(configPage);
            }
        }

    }
}

namespace ProxyManage
{
    public partial class App : Application
    {
        public App()
        {
            // 创建了一个新的 NavigationPage 实例，并将 MainPage 作为其根页面。这样做的目的是为应用程序提供导航功能，使用户能够在不同页面之间导航
            MainPage = new NavigationPage(new MainPage());

            InitializeComponent();

        }
    }
}

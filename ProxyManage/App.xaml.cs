namespace ProxyManage
{
    public partial class App : Application
    {
        public App()
        {

            MainPage = new NavigationPage(new MainPage());

            InitializeComponent();

            //MainPage = new NavigationPage(new AppShell());

        }
    }
}

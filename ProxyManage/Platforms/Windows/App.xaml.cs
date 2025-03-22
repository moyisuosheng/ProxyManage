using ProxyManage.Platforms.Windows;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ProxyManage.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{

    private static Mutex? _mutex = null;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // 确保应用程序的单实例运行
        const string appName = "ProxyManageApp";

        _mutex = new Mutex(true, appName, out bool createdNew);

        if (!createdNew)
        {
            // 如果已经有一个实例在运行，弹出提示框
            MessageUtil.ShowSuccessMessage("已经有一个 ProxyManage 程序在运行！");
            // 退出应用程序
            Environment.Exit(0);
        }

        this.InitializeComponent();

        // 注册服务依赖
        DependencyService.Register<IConfigUtil, ConfigUtil>();
        DependencyService.Register<IProxyUtil, ProxyUtil>();
        DependencyService.Register<IFolderOpener, FolderOpener>();
    }

    /// <summary>
    /// 创建 MauiApp
    /// </summary>
    /// <returns></returns>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    /// <summary>
    /// 释放资源
    /// </summary>
    public new void Exit()
    {
        try
        {
            // 释放互斥体
            _mutex?.ReleaseMutex();
            _mutex = null;
            Debug.WriteLine("释放资源");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            base.Exit();
        }
    }

}


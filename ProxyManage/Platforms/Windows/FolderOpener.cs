using WinLauncher = Windows.System.Launcher;

[assembly: Dependency(typeof(ProxyManage.Platforms.Windows.FolderOpener))]
namespace ProxyManage.Platforms.Windows
{
    public class FolderOpener : IFolderOpener
    {
        public async Task<bool> OpenFolder(string folderPath)
        {
            var folderUri = new System.Uri(folderPath);
            return await WinLauncher.LaunchFolderPathAsync(folderUri.LocalPath);
        }
    }
}

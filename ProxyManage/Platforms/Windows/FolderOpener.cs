using WinLauncher = Windows.System.Launcher;

[assembly: Dependency(typeof(ProxyManage.Platforms.Windows.FolderOpener))]
namespace ProxyManage.Platforms.Windows
{
    /// <summary>
    /// 文件夹打开器
    /// </summary>
    public class FolderOpener : IFolderOpener
    {
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public async Task<bool> OpenFolder(string folderPath)
        {
            var folderUri = new System.Uri(folderPath);
            return await WinLauncher.LaunchFolderPathAsync(folderUri.LocalPath);
        }
    }
}

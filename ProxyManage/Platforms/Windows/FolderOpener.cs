using WinLauncher = Windows.System.Launcher;

[assembly: Dependency(typeof(ProxyManage.Platforms.Windows.FolderOpener))]
namespace ProxyManage.Platforms.Windows
{
    /// <summary>
    /// �ļ��д���
    /// </summary>
    public class FolderOpener : IFolderOpener
    {
        /// <summary>
        /// ���ļ���
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

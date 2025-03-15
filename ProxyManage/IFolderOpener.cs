using ProxyManage;

/// <summary>
/// 文件夹打开器接口
/// </summary>
public interface IFolderOpener
{
    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns>是否打开</returns>
    Task<bool> OpenFolder(string folderPath);
}

using ProxyManage;

public interface IFolderOpener
{
    Task<bool> OpenFolder(string folderPath);
}

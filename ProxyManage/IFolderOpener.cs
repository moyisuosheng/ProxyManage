using ProxyManage;

/// <summary>
/// �ļ��д����ӿ�
/// </summary>
public interface IFolderOpener
{
    /// <summary>
    /// ���ļ���
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns>�Ƿ��</returns>
    Task<bool> OpenFolder(string folderPath);
}

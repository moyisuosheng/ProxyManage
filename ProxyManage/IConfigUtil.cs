using ProxyManage;

/// <summary>
/// 配置文件操作接口
/// </summary>
public interface IConfigUtil
{
    /// <summary>
    /// 获取配置文件夹路径
    /// </summary>
    /// <returns>文件夹路径</returns>
    public string GetFolderPath();

    /// <summary>
    /// 从配置文件读取配置
    /// </summary>
    /// <returns>配置集合</returns>
    public List<Config> LoadConfiguration();

    /// <summary>
    /// 保存配置到配置文件
    /// </summary>
    /// <param name="configs"></param>
    public void SaveConfiguration(List<Config> configs);
}

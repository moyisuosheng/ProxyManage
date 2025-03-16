using ProxyManage;
/// <summary>
/// 代理工具接口
/// </summary>
public interface IProxyUtil
{
    /// <summary>
    /// 设置系统代理
    /// </summary>
    /// <param name="proxy"></param>
    /// <param name="bypassList"></param>
    /// <returns></returns>
    public bool SetSystemProxy(Config config);

    /// <summary>
    /// 启用系统代理
    /// </summary>
    /// <returns></returns>
    public bool EnableSystemProxy();

    /// <summary>
    /// 禁用系统代理
    /// </summary>
    /// <returns></returns>
    public bool DisableSystemProxy();

}
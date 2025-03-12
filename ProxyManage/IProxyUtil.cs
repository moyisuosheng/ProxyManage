using ProxyManage;

public interface IProxyUtil
{

    public bool SetSystemProxy(string proxy, string bypassList);

    public bool DisableSystemProxy();

}
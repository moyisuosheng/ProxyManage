using ProxyManage;

public interface IConfigUtil
{
    public string getFolder();
    public List<Config> LoadConfiguration();

    public void SaveConfiguration(List<Config> configs);
}

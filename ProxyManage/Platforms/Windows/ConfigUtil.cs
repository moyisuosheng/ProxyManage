using System.Reflection;
using System.Text.Json;

namespace ProxyManage.Platforms.Windows
{
    public class ConfigUtil : IConfigUtil
    {
        private readonly string configFolderPath;

        private readonly string configFilePath;

        private readonly JsonSerializerOptions options;

        public ConfigUtil()
        {
            // 设置 JSON 序列化选项
            options = new() { WriteIndented = true };

            string dataPath = FileSystem.Current.AppDataDirectory;
            configFolderPath = Path.Combine(dataPath, "config");
            configFilePath = Path.Combine(configFolderPath, "config.json");

            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
            }
        }

        public string GetFolderPath()
        {
            return configFolderPath;
        }

        public List<Config> LoadConfiguration()
        {
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                return JsonSerializer.Deserialize<List<Config>>(json) ?? [];
            }
            else
            {
                // 从嵌入资源中读取默认配置文件
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("ProxyManage.Resources.Config.defaultConfig.json");
                if (null == stream)
                {
                    List<Config> list = [];
                    this.SaveConfiguration(list);
                    return list;
                }
                else
                {
                    using var reader = new StreamReader(stream);
                    var json = reader.ReadToEnd();

                    var configs = JsonSerializer.Deserialize<List<Config>>(json) ?? [];
                    this.SaveConfiguration(configs);
                    return configs;
                }

            }
        }

        public void SaveConfiguration(List<Config> configs)
        {
            var json = JsonSerializer.Serialize(configs, options);

            File.WriteAllText(configFilePath, json);
        }
    }
}

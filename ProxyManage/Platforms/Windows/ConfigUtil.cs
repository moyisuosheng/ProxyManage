using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProxyManage.Platforms.Windows
{
    public class ConfigUtil : IConfigUtil
    {
        private readonly string configFolderPath;

        private readonly string configFilePath;

        public ConfigUtil()
        {
            string dataPath = FileSystem.Current.AppDataDirectory;
            configFolderPath = Path.Combine(dataPath, "config");
            configFilePath = Path.Combine(configFolderPath, "config.json");

            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
            }
        }

        public string getFolder()
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
                using var stream = assembly.GetManifestResourceStream("ProxyManage.Resources.config.defaultConfig.json");
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

                    // 将默认配置文件写入系统临时目录
                    File.WriteAllText(configFilePath, json);
                    return configs;
                }

            }
        }

        public void SaveConfiguration(List<Config> configs)
        {
            var json = JsonSerializer.Serialize(configs);
            File.WriteAllText(configFilePath, json);
        }
    }
}

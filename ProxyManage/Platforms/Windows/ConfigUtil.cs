using System.Reflection;
using System.Text.Json;

namespace ProxyManage.Platforms.Windows
{
    /// <summary>
    /// 配置工具
    /// </summary>
    public class ConfigUtil : IConfigUtil
    {
        /// <summary>
        /// 配置文件夹路径
        /// </summary>
        private readonly string configFolderPath;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private readonly string configFilePath;

        /// <summary>
        /// JSON 序列化选项
        /// </summary>
        private readonly JsonSerializerOptions options;

        /// <summary>
        /// 构造函数
        /// </summary>
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

        /// <summary>
        /// 获取配置文件夹路径
        /// </summary>
        /// <returns></returns>
        public string GetFolderPath()
        {
            return configFolderPath;
        }

        /// <summary>
        /// 从配置文件读取配置
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 保存配置到配置文件
        /// </summary>
        /// <param name="configs"></param>
        public void SaveConfiguration(List<Config> configs)
        {
            var json = JsonSerializer.Serialize(configs, options);

            File.WriteAllText(configFilePath, json);
        }
    }
}

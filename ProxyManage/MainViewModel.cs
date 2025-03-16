using System.Collections.ObjectModel;

namespace ProxyManage
{
    public class MainViewModel
    {
        /// <summary>
        /// 配置列表
        /// </summary>
        public ObservableCollection<Config> Configs { get; set; }

        /// <summary>
        /// 配置文件操作接口
        /// </summary>
        private readonly IConfigUtil configUtil;

        public MainViewModel()
        {
            IConfigUtil iConfigUtil = DependencyService.Get<IConfigUtil>();
            if (null == iConfigUtil)
            {
                Configs = [];
                throw new Exception("IConfigUtil 未注册");
            }
            this.configUtil = iConfigUtil;
            List<Config> list = configUtil.LoadConfiguration();
            Configs = [.. list];
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfiguration()
        {
            List<Config> list = configUtil.LoadConfiguration();
            Configs = [.. list];
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public List<Config> GetConfiguration()
        {
            return [.. Configs];
        }

        /// <summary>
        /// 获取配置的索引
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public int GetIndexOfConfig(Config config)
        {
            return Configs.IndexOf(config);
        }

        /// <summary>
        /// 通过索引获取配置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Config GetConfigByIndex(int index)
        {
            if (index >= 0 && index < Configs.Count)
            {
                return Configs[index];
            }
            throw new IndexOutOfRangeException("Invalid index");
        }
        /// <summary>
        /// 更新指定索引的配置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="updatedConfig"></param>
        public void UpdateConfigByIndex(int index, Config updatedConfig)
        {
            if (index >= 0 && index < Configs.Count)
            {
                Configs[index] = updatedConfig;
                //OnPropertyChanged(nameof(Configs));
            }
        }

        /// <summary>
        /// 添加新配置
        /// </summary>
        public void AddNewConfig(Config config)
        {
            ArgumentNullException.ThrowIfNull(config);
            Configs.Add(config);
        }

        /// <summary>
        /// 移除配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool RemoveConfig(Config config)
        {
            return Configs.Remove(config);
        }

    }
}
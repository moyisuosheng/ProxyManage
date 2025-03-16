using System.Collections.ObjectModel;

namespace ProxyManage
{
    public class MainViewModel
    {
        /// <summary>
        /// �����б�
        /// </summary>
        public ObservableCollection<Config> Configs { get; set; }

        /// <summary>
        /// �����ļ������ӿ�
        /// </summary>
        private readonly IConfigUtil configUtil;

        public MainViewModel()
        {
            IConfigUtil iConfigUtil = DependencyService.Get<IConfigUtil>();
            if (null == iConfigUtil)
            {
                Configs = [];
                throw new Exception("IConfigUtil δע��");
            }
            this.configUtil = iConfigUtil;
            List<Config> list = configUtil.LoadConfiguration();
            Configs = [.. list];
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void LoadConfiguration()
        {
            List<Config> list = configUtil.LoadConfiguration();
            Configs = [.. list];
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        public List<Config> GetConfiguration()
        {
            return [.. Configs];
        }

        /// <summary>
        /// ��ȡ���õ�����
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public int GetIndexOfConfig(Config config)
        {
            return Configs.IndexOf(config);
        }

        /// <summary>
        /// ͨ��������ȡ����
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
        /// ����ָ������������
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
        /// ���������
        /// </summary>
        public void AddNewConfig(Config config)
        {
            ArgumentNullException.ThrowIfNull(config);
            Configs.Add(config);
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool RemoveConfig(Config config)
        {
            return Configs.Remove(config);
        }

    }
}
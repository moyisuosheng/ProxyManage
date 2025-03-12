using System.Collections.ObjectModel;

namespace ProxyManage
{
    public class MainViewModel
    {
        public ObservableCollection<Config> Configs { get; set; }

        public MainViewModel()
        {
            IConfigUtil configUtil = DependencyService.Get<IConfigUtil>();
            if (null == configUtil)
            {
                throw new Exception("IConfigUtil Î´×¢²á");
            }
            Configs = [.. configUtil.LoadConfiguration()];
        }

        public List<Config> getConfiguration()
        {
            return Configs.ToList();
        }
    }
}
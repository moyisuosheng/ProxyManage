namespace ProxyManage
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 代理服务器
        /// </summary>
        public string? ProxyServer { get; set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public string? BypassList { get; set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public Boolean? Local { get; set; }

    }
}

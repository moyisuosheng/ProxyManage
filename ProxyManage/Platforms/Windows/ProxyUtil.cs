using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;


namespace ProxyManage
{
    public class ProxyUtil : IProxyUtil
    {
        /// <summary>
        /// 选中“请勿将代理服务器用于本地(Intranet)地址”字符串
        /// </summary>
        private readonly string LocalStr = "<local>";

        public bool SetSystemProxy(Config config)
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                if (config == null)
                {
                    Debug.WriteLine("配置为空");
                    return false;
                }

                bool local = config.Local??false;
                string proxyServer = config.ProxyServer ?? "";
                string bypassList = config.BypassList ?? "";

                List<string> list = bypassList.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

                if (local)
                {
                    // 判断集合中是否存在值与 LocalStr 相同的元素
                    if (list.Contains(LocalStr))
                    {
                        Debug.WriteLine("已经包含<local>，无需添加");
                    }
                    else
                    {

                        list.Add(LocalStr);
                        bypassList = string.Join(";", list);
                    }
                }
                else
                {
                    list.RemoveAll(item => item == LocalStr);
                    bypassList = string.Join(";", list);
                }

                return SetWindowsProxy(proxyServer, bypassList);
            }
            else
            {
                Debug.WriteLine("当前平台不支持设置系统代理");
            }
            return false;
        }

        public bool DisableSystemProxy()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return DisableWindowsProxy();

            }
            else
            {
                Debug.WriteLine("当前平台不支持设置系统代理");
            }
            return false;
        }

        private bool SetWindowsProxy(string proxy, string bypassList)
        {
            var proxyInfo = new INTERNET_PER_CONN_OPTION_LIST();
            var options = new INTERNET_PER_CONN_OPTION[3];

            options[0] = new INTERNET_PER_CONN_OPTION
            {
                dwOption = INTERNET_PER_CONN_FLAGS,
                Value = new INTERNET_PER_CONN_OPTION_UNION
                {
                    dwValue = PROXY_TYPE_DIRECT | PROXY_TYPE_PROXY
                }
            };

            options[1] = new INTERNET_PER_CONN_OPTION
            {
                dwOption = INTERNET_PER_CONN_PROXY_SERVER,
                Value = new INTERNET_PER_CONN_OPTION_UNION
                {
                    pszValue = Marshal.StringToHGlobalAnsi(proxy)
                }
            };

            options[2] = new INTERNET_PER_CONN_OPTION
            {
                dwOption = INTERNET_PER_CONN_PROXY_BYPASS,
                Value = new INTERNET_PER_CONN_OPTION_UNION
                {
                    pszValue = Marshal.StringToHGlobalAnsi(bypassList)
                }
            };

            proxyInfo.dwSize = Marshal.SizeOf(proxyInfo);
            proxyInfo.pszConnection = IntPtr.Zero;
            proxyInfo.dwOptionCount = options.Length;
            proxyInfo.dwOptionError = 0;

            var optionSize = Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION)) * options.Length;
            proxyInfo.pOptions = Marshal.AllocCoTaskMem(optionSize);
            Marshal.StructureToPtr(options[0], proxyInfo.pOptions, false);
            Marshal.StructureToPtr(options[1], proxyInfo.pOptions + Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION)), false);
            Marshal.StructureToPtr(options[2], proxyInfo.pOptions + 2 * Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION)), false);

            var result = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PER_CONNECTION_OPTION, ref proxyInfo, Marshal.SizeOf(proxyInfo));
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            Marshal.FreeHGlobal(options[1].Value.pszValue);
            Marshal.FreeHGlobal(options[2].Value.pszValue);
            Marshal.FreeCoTaskMem(proxyInfo.pOptions);

            if (result)
            {

                Debug.WriteLine("成功设置代理");

            }
            else
            {

                Debug.WriteLine("设置代理失败");
            }
            return result;
        }

        private bool DisableWindowsProxy()
        {
            var proxyInfo = new INTERNET_PER_CONN_OPTION_LIST();
            var options = new INTERNET_PER_CONN_OPTION[1];

            options[0] = new INTERNET_PER_CONN_OPTION
            {
                dwOption = INTERNET_PER_CONN_FLAGS,
                Value = new INTERNET_PER_CONN_OPTION_UNION
                {
                    dwValue = PROXY_TYPE_DIRECT
                }
            };

            proxyInfo.dwSize = Marshal.SizeOf(proxyInfo);
            proxyInfo.pszConnection = IntPtr.Zero;
            proxyInfo.dwOptionCount = options.Length;
            proxyInfo.dwOptionError = 0;

            var optionSize = Marshal.SizeOf(typeof(INTERNET_PER_CONN_OPTION)) * options.Length;
            proxyInfo.pOptions = Marshal.AllocCoTaskMem(optionSize);
            Marshal.StructureToPtr(options[0], proxyInfo.pOptions, false);

            var result = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PER_CONNECTION_OPTION, ref proxyInfo, Marshal.SizeOf(proxyInfo));
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            Marshal.FreeCoTaskMem(proxyInfo.pOptions);

            if (result)
            {
                Debug.WriteLine("成功禁用代理");
            }
            else
            {
                Debug.WriteLine("禁用代理失败");
            }
            return result;
        }

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, ref INTERNET_PER_CONN_OPTION_LIST lpBuffer, int dwBufferLength);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct INTERNET_PER_CONN_OPTION_LIST
        {
            public int dwSize;
            public IntPtr pszConnection;
            public int dwOptionCount;
            public int dwOptionError;
            public IntPtr pOptions;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INTERNET_PER_CONN_OPTION
        {
            public int dwOption;
            public INTERNET_PER_CONN_OPTION_UNION Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INTERNET_PER_CONN_OPTION_UNION
        {
            [FieldOffset(0)]
            public int dwValue;
            [FieldOffset(0)]
            public IntPtr pszValue;
            [FieldOffset(0)]
            public FILETIME ftValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        private const int INTERNET_OPTION_PER_CONNECTION_OPTION = 75;
        private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        private const int INTERNET_OPTION_REFRESH = 37;
        private const int INTERNET_PER_CONN_FLAGS = 1;
        private const int INTERNET_PER_CONN_PROXY_SERVER = 2;
        private const int INTERNET_PER_CONN_PROXY_BYPASS = 3;
        private const int PROXY_TYPE_DIRECT = 0x00000001;
        private const int PROXY_TYPE_PROXY = 0x00000002;

    }
}

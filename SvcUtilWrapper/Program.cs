using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;

namespace SvcUtilWrapper
{
    /// <summary>
    /// Wraps SvcUtil.exe to allow controlling the security protocol used.
    /// https://stackoverflow.com/a/51795160/1594171
    /// </summary>
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            if (!Enum.TryParse(ConfigurationManager.AppSettings["SecurityProtocolType"], ignoreCase: true, out SecurityProtocolType securityProtocolType))
            {
                throw new InvalidOperationException("Could not read SecurityProtocolType in app config, make sure this key is present and contains a valid value for SecurityProtocolType.");
            }

            var fullPathToSvcUtilExe = ConfigurationManager.AppSettings["FullPathToSvcUtilExe"];
            if (string.IsNullOrWhiteSpace(fullPathToSvcUtilExe))
            {
                throw new InvalidOperationException("Could not read FullPathToSvcUtilExe in app config, make sure this key is present and contains a value containing the fully qualified path to SvcUtil.exe");
            }
            
            if (!File.Exists(fullPathToSvcUtilExe))
            {
                throw new InvalidOperationException($"Could not find SvcUtil.exe at this path: {fullPathToSvcUtilExe}");
            }
            
            ServicePointManager.SecurityProtocol = securityProtocolType;
            var svcUtilAssembly = Assembly.LoadFile(fullPathToSvcUtilExe);
            svcUtilAssembly.EntryPoint?.Invoke(null, new object[] { args });
        }
    }
}

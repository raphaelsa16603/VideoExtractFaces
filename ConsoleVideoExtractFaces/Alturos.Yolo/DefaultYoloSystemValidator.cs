using Alturos.Yolo.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Alturos.Yolo
{
    public class DefaultYoloSystemValidator : IYoloSystemValidator
    {
        public SystemValidationReport Validate()
        {
            var report = new SystemValidationReport();

#if NETSTANDARD
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                report.MicrosoftVisualCPlusPlusRedistributableExists = this.IsMicrosoftVisualCPlusPlusAvailable();
            }
            else
            {
                report.MicrosoftVisualCPlusPlusRedistributableExists = true;
            }
#endif

#if NET461
            report.MicrosoftVisualCPlusPlusRedistributableExists = this.IsMicrosoftVisualCPlusPlusAvailable();
#endif

            if (File.Exists("cudnn64_7.dll"))
            {
                report.CudnnExists = true;
            }

            var environmentVariables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);
            if (environmentVariables.Contains("CUDA_PATH"))
            {
                report.CudaExists = true;
            }
            if (environmentVariables.Contains("CUDA_PATH_V10_2"))
            {
                report.CudaExists = true;
            }

            return report;
        }

        private bool IsMicrosoftVisualCPlusPlusAvailable()
        {
            // Dicionário com as versões relevantes do Visual C++ Redistributable (x64)
            var checkKeys = new Dictionary<string, List<string>>
            {
                { @"Installer\Dependencies\,,amd64,14.0,bundle", new List<string> { "Microsoft Visual C++ 2017 Redistributable (x64)" } },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.16,bundle", new List<string> { "Microsoft Visual C++ 2017 Redistributable (x64)" } },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.20,bundle", new List<string>
                    {
                        "Microsoft Visual C++ 2015-2019 Redistributable (x64)",
                        "Microsoft Visual C++ 2017 Redistributable (x64)"
                    }
                },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.21,bundle", new List<string> { "Microsoft Visual C++ 2015-2019 Redistributable (x64)" } },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.22,bundle", new List<string> { "Microsoft Visual C++ 2015-2019 Redistributable (x64)" } },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.23,bundle", new List<string> { "Microsoft Visual C++ 2015-2019 Redistributable (x64)" } },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.24,bundle", new List<string> { "Microsoft Visual C++ 2015-2019 Redistributable (x64)" } },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.25,bundle", new List<string> { "Microsoft Visual C++ 2015-2019 Redistributable (x64)" } },
                { @"Installer\Dependencies\VC,redist.x64,amd64,14.26,bundle", new List<string> { "Microsoft Visual C++ 2015-2019 Redistributable (x64)" } }
            };

            foreach (var checkKey in checkKeys)
            {
                using (var registryKey = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Classes\{checkKey.Key}", false))
                {
                    if (registryKey == null)
                    {
                        continue;
                    }

                    var displayName = registryKey.GetValue("DisplayName") as string;
                    if (string.IsNullOrEmpty(displayName))
                    {
                        continue;
                    }

                    foreach (var version in checkKey.Value)
                    {
                        if (displayName.StartsWith(version, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}

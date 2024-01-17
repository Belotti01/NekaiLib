using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public static partial class CurrentEnvironment
{
    public static class App
    {
        public static Configuration ReadConfiguration(ConfigurationUserLevel level = ConfigurationUserLevel.PerUserRoamingAndLocal)
        {
            return ConfigurationManager
                .OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
        }

        /// <inheritdoc cref="GCMemoryInfo.TotalCommittedBytes"/>
        public static long GetUsedMemory()
        {
            return GC.GetGCMemoryInfo().TotalCommittedBytes;
        }

        /// <summary>
        /// Check whether the memory usage of the last GC invocation is above the "High-load" threshold.
        /// </summary>
        public static bool IsMemoryUnderPressure()
        {
            var memoryInfo = GC.GetGCMemoryInfo();
            return memoryInfo.MemoryLoadBytes > memoryInfo.HighMemoryLoadThresholdBytes;
        }
    }
}

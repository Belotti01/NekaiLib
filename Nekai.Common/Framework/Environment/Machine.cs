using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common.Framework.ToolKit;

public static partial class CurrentEnvironment
{
    public static class Machine
    {
        public static Configuration ReadConfiguration()
            => ConfigurationManager
                .OpenMachineConfiguration();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;

namespace Nekai.Razor;

public class NekaiDesignTheme : FluentDesignTheme
{
    public NekaiDesignTheme()
    {
        // Set defaults
        StorageName = "theme";
        Mode = DesignThemeModes.System;
        CustomColor = "#4dbafe";
    }

}

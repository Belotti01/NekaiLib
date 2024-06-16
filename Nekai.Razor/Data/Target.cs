using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Razor;

public enum Target
{
    Self,
    Blank,
    Parent,
    Top
}

public static class TargetExtensions
{
    public static string ToTargetString(this Target target)
    {
        return target switch
        {
            Target.Blank => "_blank",
            Target.Parent => "_parent",
            Target.Top => "_top",
            _ => "_self",
        };
    }
}

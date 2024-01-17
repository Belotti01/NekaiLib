using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public static class NumberOperationExtensions
{

    public static T AddWithoutOverflow<T>(this T value, T other)
        where T : IMinMaxValue<T>, ISubtractionOperators<T, T, T>, INumber<T>, INumberBase<T>
    {
        if(T.MaxValue - other > value)
            return value + other;
        return T.MaxValue;
    }
}

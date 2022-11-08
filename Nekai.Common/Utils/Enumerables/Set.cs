using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common.Utils.Enumerables;

public class Set<T> : HashSet<T> {
    
    
    public static Set<T> operator |(Set<T> a, IEnumerable<T> b)
    {
        Set<T> set = new();
        set.UnionWith(a);
        set.UnionWith(b);
        return set;
    }
    
    public static Set<T> operator &(Set<T> a, IEnumerable<T> b)
    {
        Set<T> set = new();
        set.UnionWith(a);
        set.IntersectWith(b);
        return set;
    }

    public static Set<T> operator -(Set<T> a, IEnumerable<T> b)
    {
        Set<T> set = new();
        set.UnionWith(a);
        set.ExceptWith(b);
        return set;
    }

    public static Set<T> operator ^(Set<T> a, IEnumerable<T> b)
    {
        Set<T> set = new();
        set.UnionWith(a);
        set.SymmetricExceptWith(b);
        return set;
    }

    public static Set<T> operator +(Set<T> a, T b)
    {
        Set<T> set = new();
        set.UnionWith(a);
        set.Add(b);
        return set;
    }

    public static Set<T> operator -(Set<T> a, T b)
    {
        Set<T> set = new();
        set.UnionWith(a);
        set.Remove(b);
        return set;
    }

    /// <summary>
    /// Generate a <see langword="string"/> displaying the elements of this <see cref="Set{T}"/> in 
    /// Set Notation.
    /// </summary>
    public override string ToString()
    {
        return $"{{ {string.Join(", ", this)} }}";
    }

}

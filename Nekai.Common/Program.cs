using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nekai.Common.Utils.Enumerables;

namespace Nekai.Common;
internal class Program {
    
    public static void Main(string[] args)
    {
        Set<int> a = new() { 1, 2, 3, 4, 5 };
        Set<int> b = new() { 4, 5, 6, 7, 8 };

        Console.WriteLine(a + 10);
        Console.WriteLine(a | b);
        Console.WriteLine(a & b);
        Console.WriteLine(a ^ b);
        Console.WriteLine(a - b);
    }
}

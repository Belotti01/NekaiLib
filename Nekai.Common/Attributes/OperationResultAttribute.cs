using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;


/// <summary>
/// An attribute that indicates that the enum identifies an operation result.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class OperationResultAttribute : Attribute
{
}

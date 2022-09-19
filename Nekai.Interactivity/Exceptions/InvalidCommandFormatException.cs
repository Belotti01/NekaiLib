using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nekai;

public class InvalidCommandFormatException : Exception {
	public InvalidCommandFormatException() : base("Commands must be static and either parameterless or accepting a nullable string parameter.") { }
	public InvalidCommandFormatException(string commandName) : base($"Command \"{commandName}\" must be static and either parameterless or accepting a nullable string parameter.") { }
	public InvalidCommandFormatException(MethodInfo method) : base($"Command \"{method.Name}\" must be static and either parameterless or accepting a nullable string parameter.") { }
}

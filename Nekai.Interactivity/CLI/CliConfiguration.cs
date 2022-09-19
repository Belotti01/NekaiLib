using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Interactivity;

public class CliConfiguration {
	public bool DeleteRequestAfterValidInput { get; set; } = true;
	public char DefaultDivisorCharacter { get; set; } = '~';
	public int DefaultMinimumDivisorLength { get; set; } = 10;
	
	public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;
	public ConsoleColor WarningColor { get; set; } = ConsoleColor.Yellow;
	public ConsoleColor SuccessColor { get; set; } = ConsoleColor.Green;
	public ConsoleColor InformationColor { get; set; } = ConsoleColor.Cyan;
	public ConsoleColor RequestMessageColor { get; set; } = ConsoleColor.Yellow;
	public ConsoleColor DivColor { get; set; } = ConsoleColor.Cyan;
}

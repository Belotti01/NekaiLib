using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nekai.Analyzers.Test
{
	[TestClass]
	public class NekaiAnalyzersUnitTest
	{
		[TestMethod]
		public async Task TestMethod1()
		{
			var test = @"";

			await VerifyCS.VerifyAnalyzerAsync(test);
		}
	}
}

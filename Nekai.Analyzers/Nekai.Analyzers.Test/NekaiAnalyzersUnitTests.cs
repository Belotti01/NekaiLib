using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Nekai.Analyzers.Test.CSharpCodeFixVerifier<
	Nekai.Analyzers.NekaiAnalyzer,
	Nekai.Analyzers.NekaiAnalyzersCodeFixProvider>;

namespace Nekai.Analyzers.Test
{
	[TestClass]
	public class NekaiAnalyzersUnitTest
	{
		//No diagnostics expected to show up
		[TestMethod]
		public async Task TestMethod1()
		{
			var test = @"";

			await VerifyCS.VerifyAnalyzerAsync(test);
		}

		//Diagnostic and CodeFix both triggered and checked for
		[TestMethod]
		public void TestMethod2()
		{
			
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nekai.Analyzers.Test.Tests.Enums;

[TestClass]
public class OperationResultBaseTypeTests
{

	[TestMethod("With no base type")]
	public async Task WithoutBaseType()
	{
		var test = @"namespace Test { 
public class OperationResultAttribute : System.Attribute { }
[OperationResult] public enum TestOperationResult { }
}";

		await VerifyCS.VerifyAnalyzerAsync(test);
	}

	[TestMethod("With Int32 base type")]
	public async Task WithInt32BaseType()
	{
		var test = @"namespace Test { 
public class OperationResultAttribute : System.Attribute { }
[OperationResult] public enum TestOperationResult : int { }
}";
		await VerifyCS.VerifyAnalyzerAsync(test);
	}


	[TestMethod("With byte base type")]
	public async Task WithByteBaseType()
	{
		var test = @"namespace Test { 
public class OperationResultAttribute : System.Attribute { }
[OperationResult] public enum TestOperationResult : byte { }
}";
		var expected = VerifyCS.Diagnostic(NekaiDiagnostics.OperationResultBaseType.AsRule)
			.WithLocationOf(test, ": byte", "TestOperationResult")
			.WithArguments("TestOperationResult", nameof(Byte));
		await VerifyCS.VerifyAnalyzerAsync(test, expected);
	}


	[TestMethod("Fix byte base type")]
	public async Task CodeFixByteBaseType()
	{
		var test = @"namespace Test { 
public class OperationResultAttribute : System.Attribute { }
[OperationResult] public enum TestOperationResult : byte { }
}";
		string expected = test.Replace(": byte ", "");
		await VerifyCS.VerifyCodeFixAsync(test, expected);
	}
}

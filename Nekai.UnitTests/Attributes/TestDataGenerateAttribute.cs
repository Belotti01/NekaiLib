namespace Nekai.UnitTests;

public class TestDataGenerateAttribute : TestCaseSourceAttribute
{
	public TestDataGenerateAttribute(params Type[] paramsTypes)
		: base(typeof(TestData), nameof(TestData.Generate), new object[] { paramsTypes })
	{
	}
}
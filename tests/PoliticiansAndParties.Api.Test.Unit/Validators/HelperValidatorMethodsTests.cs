using PoliticiansAndParties.Api.Validators;

namespace PoliticiansAndParties.Api.Test.Unit.Validators;

public class HelperValidatorMethodsTests
{
    [Theory]
    [InlineData("", false)]
    [InlineData("karel", false)]
    [InlineData("smtp://test.com", false)]
    [InlineData("http://test.com", true)]
    [InlineData("https://test.com", true)]
    [InlineData("https//test.com", false)]
    [InlineData("https:/test.com", false)]
    [InlineData("https://test.com/image?test=photo", true)]
    public void IsValidUrlTests(string url, bool result) => HelperValidatorMethods.IsValidUrl(url).Should().Be(result);
}

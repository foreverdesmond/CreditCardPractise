using CreditCard.BusinessLogic.Utilities;
using Xunit;

namespace CreditCard.BusinessLogic.Tests.Utilities
{
    public class IsValidLuhnTests
    {
        [Theory]
        [InlineData("4532015112830366", true)] // Valid Visa
        [InlineData("6011514433546201", true)] // Valid Discover
        [InlineData("378282246310005", true)]  // Valid American Express
        [InlineData("371449635398431", true)]  // Valid American Express
        [InlineData("5111111111111118", true)] // Valid MasterCard
        [InlineData("1234567812345678", false)] // Invalid
        [InlineData("4532015112830367", false)] // Invalid
        [InlineData("6011514433546202", false)] // Invalid
        [InlineData("", false)] // Empty string
        [InlineData(" ", false)] // Whitespace
        [InlineData(null, false)] // Null
        [InlineData("abcd1234", false)] // Non-digit characters
        public void Validate_ShouldReturnExpectedResult(string cardNumber, bool expected)
        {
            // Act
            bool result = IsValidLuhn.Validate(cardNumber);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
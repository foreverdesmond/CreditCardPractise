using CreditCard.BusinessLogic.Services;
using CreditCard.Models.DTOs;
using Xunit;

namespace CreditCard.BusinessLogic.Tests
{
    public class CreditCardServiceTests
    {
        private readonly CreditCardService _creditCardService;

        public CreditCardServiceTests()
        {
            _creditCardService = new CreditCardService();
        }

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
        public void IsValidCardNumber_ShouldReturnExpectedResult(string cardNumber, bool expected)
        {
            // Act
            bool result = _creditCardService.IsValidCardNumber(cardNumber);

            // Assert
            Assert.Equal(expected, result);
        }

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
        public void IsValidCardNumber_WithDto_ShouldReturnExpectedResult(string cardNumber, bool expected)
        {
            // Arrange
            var creditCardDto = new CreditCardDto
            {
                Id = Guid.NewGuid(),
                CardNumber = cardNumber,
                CardHolderName="TestUser",
                ExpiryMonth="06",
                ExpiryYear="26",
                CVV="888",
                Issuer="Visa"
            };

            // Act
            bool result = _creditCardService.IsValidCardNumber(creditCardDto);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
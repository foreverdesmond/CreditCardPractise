using CreditCard.API.Controllers;
using CreditCard.Models.DTOs;
using CreditCard.API.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Reflection;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace CreditCard.BusinessLogic.Tests.Controllers
{
    public class CreditCardControllerTests
    {
        private readonly CreditCardController _controller;
        private readonly ModelValidationFilter _validationFilter;

        public CreditCardControllerTests()
        {
            _validationFilter = new ModelValidationFilter();
            _controller = new CreditCardController();
        }

        private void ValidateModel(object model)
        {
            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerName = "CreditCard",
                ActionName = "CreditCardIsValid",
                ControllerTypeInfo = typeof(CreditCardController).GetTypeInfo(),
                Parameters = new List<ParameterDescriptor>()
            };

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                actionDescriptor);

            var modelState = new ModelStateDictionary();
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            // 执行验证
            var isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            if (!isValid)
            {
                foreach (var validationResult in results)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        modelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }

            // 对于 CreditCardDto，添加特定的验证
            if (model is CreditCardDto dto)
            {
                // 验证 ExpiryMonth 是否为有效的数字
                if (!int.TryParse(dto.ExpiryMonth, out int month) || month < 1 || month > 12)
                {
                    modelState.AddModelError("ExpiryMonth", "Invalid expiry month");
                }

                // 验证 ExpiryYear 是否为有效的数字
                if (!int.TryParse(dto.ExpiryYear, out int year))
                {
                    modelState.AddModelError("ExpiryYear", "Invalid expiry year");
                }

                // 验证 CVV 是否为有效的数字
                if (!dto.CVV.All(char.IsDigit))
                {
                    modelState.AddModelError("CVV", "CVV must contain only digits");
                }
            }

            _controller.ControllerContext = new ControllerContext(actionContext);
            _controller.ModelState.Clear();
            foreach (var (key, value) in modelState)
            {
                foreach (var error in value.Errors)
                {
                    _controller.ModelState.AddModelError(key, error.ErrorMessage);
                }
            }

            // 执行 ModelValidationFilter
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                _controller);
            
            _validationFilter.OnActionExecuting(actionExecutingContext);
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
        public async Task CreditCardIsValid_ShouldReturnExpectedResult(string cardNumber, bool expected)
        {
            // Arrange
            ValidateModel(cardNumber);

            // Act
            var result = await _controller.CreditCardIsValid(cardNumber);

            // Assert
            Assert.NotNull(result);
            if (!_controller.ModelState.IsValid)
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
            else if (expected)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = okResult.Value as dynamic;
                Assert.Equal(expected, response.IsValid);
            }
            else
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Theory]
        [InlineData("4532015112830366", "Tom", "0b", "21", "054", false)]  // Invalid month
        [InlineData("4532015112830366", "Tom", "05", "aa", "054", false)]  // Invalid year
        [InlineData("4532015112830366", "Tom", "05", "24", "aaa", false)]  // Invalid CVV
        public async Task CreditCardIsValid_WithDtoErrors_ShouldReturnExpectedResult(
            string cardNumber, string cardHolderName, string expiryMonth, 
            string expiryYear, string cvv, bool expected)
        {
            // Arrange
            var creditCardDto = new CreditCardDto
            {
                CardNumber = cardNumber,
                CardHolderName = cardHolderName,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                CVV = cvv,
                Issuer = "Visa"
            };

            ValidateModel(creditCardDto);

            // Act
            var result = await _controller.CreditCardIsValid(creditCardDto);

            // Assert
            Assert.NotNull(result);
            if (!_controller.ModelState.IsValid)
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
            else if (expected)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = okResult.Value as dynamic;
                Assert.Equal(expected, response.IsValid);
            }
            else
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
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
        public async Task CreditCardIsValid_WithDto_ShouldReturnExpectedResult(string cardNumber, bool expected)
        {
            // Arrange
            var creditCardDto = new CreditCardDto
            {
                CardNumber = cardNumber,
                CardHolderName = "TestUser",
                ExpiryMonth = "06",
                ExpiryYear = "26",
                CVV = "888",
                Issuer = "Visa"
            };
            ValidateModel(creditCardDto);

            // Act
            var result = await _controller.CreditCardIsValid(creditCardDto);

            // Assert
            Assert.NotNull(result);
            if (!_controller.ModelState.IsValid)
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
            else if (expected)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = okResult.Value as dynamic;
                Assert.Equal(expected, response.IsValid);
            }
            else
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }
    }
}

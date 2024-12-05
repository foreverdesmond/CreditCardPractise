using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;
using CreditCard.Models.DTOs;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using CreditCard.API;
using System.Net;
using System;
using System.Threading.Tasks;
using NLog;
using System.Text.Json;

namespace CreditCard.API.Tests.Controllers
{

    public class CreditCardControllerIntegrationTests
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly HttpClient _client;

        public CreditCardControllerIntegrationTests()
        {
            var apiBaseAddress = "https://localhost:7021";
            _client = new HttpClient
            {
                BaseAddress = new Uri(apiBaseAddress)
            };
        }

        [Fact]
        public async Task EnsureApiIsRunning()
        {
            Logger.Info("Starting EnsureApiIsRunning test.");
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();
            Logger.Info($"Response Status: {response.StatusCode}, Content: {content}");
            Assert.True(response.IsSuccessStatusCode, $"API is not running. Status: {response.StatusCode}, Content: {content}");
        }

        [Theory]
        [InlineData("4532015112830366", HttpStatusCode.OK,true)] // Valid Visa
        [InlineData("6011514433546201", HttpStatusCode.OK, true)] // Valid Discover
        [InlineData("378282246310005", HttpStatusCode.OK, true)]  // Valid American Express
        [InlineData("371449635398431", HttpStatusCode.OK, true)]  // Valid American Express
        [InlineData("5111111111111118", HttpStatusCode.OK, true)] // Valid MasterCard
        [InlineData("1234567812345678", HttpStatusCode.OK, false)] // Invalid
        [InlineData("4532015112830367", HttpStatusCode.OK,false)] // Invalid
        [InlineData("6011514433546202", HttpStatusCode.OK, false)] // Invalid
        [InlineData("", HttpStatusCode.BadRequest, false)] // Empty string
        [InlineData(" ", HttpStatusCode.BadRequest, false)] // Whitespace
        [InlineData(null, HttpStatusCode.BadRequest, false)] // Null
        [InlineData("abcd1234", HttpStatusCode.BadRequest, false)] // Non-digit characters
        public async Task CreditCardIsValid_ShouldReturnExpectedResult(string cardNumber, HttpStatusCode httpStatusCode, bool isValid)
        {
             Logger.Info($"Starting CreditCardIsValid test with cardNumber: {cardNumber}");
            try
            {
                var requestUrl = $"api/creditcard/validate/card-number?cardNumber={cardNumber}";
                Logger.Info($"Request URL: {_client.BaseAddress}{requestUrl}");

                var response = await _client.GetAsync(requestUrl);
                Logger.Info($"Response Status: {response.StatusCode}");

                if ( httpStatusCode == HttpStatusCode.OK)
                {
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    var isValidResult = result.GetProperty("isValid").GetBoolean();
                    Logger.Info($"Validation Result: {isValidResult}");
                    Assert.Equal(isValidResult, isValid);
                }
                else
                {
                    Assert.Equal(httpStatusCode, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Test failed with exception.");
                Assert.True(false, $"Test failed with exception: {ex.Message}");
            }
        }

        [Theory]
        [InlineData("4532015112830366", "Tom", "06", "25", "123", HttpStatusCode.OK, true)]  // Valid
        [InlineData("4532015112830366", "Tom", "13", "25", "123", HttpStatusCode.BadRequest, false)] // Invalid month
        [InlineData("4532015112830366", "Tom", "06", "cd", "123", HttpStatusCode.BadRequest,false)] // Invalid year
        [InlineData("4532015112830366", "Tom", "06", "25", "abc", HttpStatusCode.BadRequest,false)] // Invalid CVV
        public async Task CreditCardIsValid_WithFullDetails_ShouldReturnExpectedResult(
            string cardNumber, string cardHolderName, string expiryMonth,
            string expiryYear, string cvv, HttpStatusCode httpStatusCode, bool isValid)
        {
            Logger.Info($"Starting CreditCardIsValid_WithFullDetails test with cardNumber: {cardNumber}");
            try
            {
                var creditCardDto = new CreditCardDto
                {
                    CardNumber = cardNumber,
                    CardHolderName = cardHolderName,
                    ExpiryMonth = expiryMonth,
                    ExpiryYear = expiryYear,
                    CVV = cvv,
                    Issuer = "Visa"
                };

                Logger.Info($"Request URL: {_client.BaseAddress}api/creditcard/validate/credit-card");
                Logger.Info($"Request Body: {JsonSerializer.Serialize(creditCardDto)}");

                var response = await _client.PostAsJsonAsync("api/creditcard/validate/credit-card", creditCardDto);
                Logger.Info($"Response Status: {response.StatusCode}");

                if (httpStatusCode == HttpStatusCode.OK)
                {
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    var isValidResult = result.GetProperty("isValid").GetBoolean();
                    Logger.Info($"Validation Result: {isValidResult}");
                    Assert.Equal(isValidResult, isValid);
                }
                else
                {
                    Assert.Equal(httpStatusCode, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Test failed with exception.");
                Assert.True(false, $"Test failed with exception: {ex.Message}");
            }
        }
    }
}
using CreditCard.API.Filters;
using CreditCard.BusinessLogic.Factories;
using CreditCard.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.ComponentModel.DataAnnotations;

namespace CreditCard.API.Controllers
{

    /// <summary>
    /// Handles API requests related to credit cards.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CreditCardController : ControllerBase
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Validates the provided credit card number for its validity.
        /// </summary>
        /// <param name="cardNumber">The credit card number, which must be a numeric string of 13 to 16 digits.</param>
        /// <returns>Returns the credit card number along with its validity status.</returns>
        /// <response code="200">Returns valid credit card information including the card number and its validity status.</response>
        /// <response code="400">If the credit card number is invalid or incorrectly formatted.</response>
        /// <response code="500">If the credit card service is unavailable.</response>
        [HttpGet("validate/card-number")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreditCardIsValid([Required, RegularExpression(@"^\d{13,16}$", ErrorMessage = "Card number must be a 13 to 16-digit number.")] string cardNumber)
        {
            var service = ServiceFactory.Create("CreditCardService");

            if (service == null)
            {
                Logger.Error("CreditCard service is not available.");
                return StatusCode(500,"CreditCard service is not available.");
            }

            bool isValid = service.IsValidCardNumber(cardNumber);

            return Ok(new
            {
                CardNumber = cardNumber,
                IsValid = isValid
            });
        }

        /// <summary>
        /// Validates the credit card DTO for its validity.
        /// </summary>
        /// <param name="creditCardDto">The DTO containing credit card information, including card number, holder name, expiry date, and CVV.</param>
        /// <returns>Returns the credit card number along with its validity status.</returns>
        /// <response code="200">Returns valid credit card information including the card number and its validity status.</response>
        /// <response code="400">If the credit card information is invalid or incorrectly formatted.</response>
        /// <response code="500">If there is an internal server error, such as service unavailability.</response>
        [HttpPost("validate/credit-card")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreditCardIsValid([FromBody] CreditCardDto creditCardDto)
        {
            var service = ServiceFactory.Create("CreditCardService");

            if (service == null)
            {
                Logger.Error("CreditCard service is not available.");
                return StatusCode(500, "CreditCard service is not available.");
            }

            bool isValid = service.IsValidCardNumber(creditCardDto.CardNumber);

            return Ok(new
            {
                CardNumber = creditCardDto.CardNumber,
                IsValid = isValid
            });
        }
    }
}
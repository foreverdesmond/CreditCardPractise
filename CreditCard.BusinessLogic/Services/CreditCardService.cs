using CreditCard.BusinessLogic.Utilities;
using CreditCard.Models.DTOs;

namespace CreditCard.BusinessLogic.Services
{
    public class CreditCardService : ICreditCardService
    {
        public bool IsValidCardNumber(string cardNumber)
        {
            return IsValidLuhn.Validate(cardNumber);
        }

        public bool IsValidCardNumber(CreditCardDto creditCardDto)
        {
            return IsValidLuhn.Validate(creditCardDto.CardNumber);
        }
    }
}
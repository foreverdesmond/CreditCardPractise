using CreditCard.Models.DTOs;

namespace CreditCard.BusinessLogic.Services
{
    public interface ICreditCardService
    {
        bool IsValidCardNumber(string cardNumber);

        bool IsValidCardNumber(CreditCardDto creditCardDto);
    }
}
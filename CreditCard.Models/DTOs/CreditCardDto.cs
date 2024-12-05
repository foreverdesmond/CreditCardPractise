using System.ComponentModel.DataAnnotations;

namespace CreditCard.Models.DTOs
{
    public class CreditCardDto
    {
        //[Required(ErrorMessage = "Card ID is required.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^\d{13,16}$", ErrorMessage = "Card number must be numeric and between 13 and 16 digits.")]
        public required string CardNumber { get; set; }

        [Required(ErrorMessage = "Card holder name is required.")]
        [StringLength(100, ErrorMessage = "Card holder name must not exceed 100 characters.")]
        public required string CardHolderName { get; set; }

        [Required(ErrorMessage = "Expiry month is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Expiry month must be a two-digit number between 01 and 12.")]
        public required string ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Expiry year is required.")]
        [RegularExpression(@"^(0[0-9]|[1-9][0-9])$", ErrorMessage = "Expiry year must be a two-digit number between 00 and 99.")]
        public required string ExpiryYear { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be exactly 3 numeric characters.")]
        public required string CVV { get; set; }

        [StringLength(50, ErrorMessage = "Issuer must not exceed 50 characters.")]
        public string? Issuer { get; set; }

        public DateTimeOffset ExpiryDate
        {
            get
            {
                try
                {
                    int fullYear = 2000 + Convert.ToInt32(ExpiryYear);
                    int expiryMonth = Convert.ToInt32(ExpiryMonth);
                    if (expiryMonth == 12)
                    {
                        expiryMonth = 1;
                        fullYear++;
                    }

                    TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
                    return new DateTimeOffset(fullYear, expiryMonth, 1, 0, 0, 0, offset);
                }
                catch (Exception ex)
                {
                    return new DateTimeOffset();
                }
            }
        }
    }
}
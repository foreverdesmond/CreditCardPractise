using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCard.Models
{
    [Table("CreditCards")]
    public class CreditCard
    {
        [Key]
        [Column("guid")]
        public Guid Id { get; set; }

        [Column("card_number")]
        public required string CardNumber { get; set; }

        [Column("card_holder_name")]
        public required string CardHolderName { get; set; }

        [Column("expiry_month")]
        public string ExpiryMonth { get; set; }

        [Column("expiry_year")]
        public string ExpiryYear { get; set; }

        [Column("ccv")]
        public required string CVV { get; set; }

        [Column("issuer")]
        public string? Issuer { get; set; }

        [Column("expire_datetime")]
        public DateTimeOffset ExpiryDate { get; set; }
    }
}


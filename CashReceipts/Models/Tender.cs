using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{

    public class Tender
    {
        public int TenderID { get; set; }

        [StringLength(30, ErrorMessage = "Description  name cannot be longer than 30 characters.")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Amount")]
        [Required]
        public decimal Amount { get; set; }

        public int ReceiptHeaderID { get; set; }

        public virtual ReceiptHeader ReceiptHeader { get; set; }

        public int PaymentMethodId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}
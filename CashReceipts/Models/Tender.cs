using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashReceipts.Models
{

    public class Tender
    {
        public int TenderID { get; set; }

        [StringLength(30, ErrorMessage = "Description  name cannot be longer than 30 characters.")]
        public string Description { get; set; }

        [Display(Name = "Amount")]
        [Required]
        public decimal Amount { get; set; }

        public int ReceiptHeaderID { get; set; }

        public virtual ReceiptHeader ReceiptHeader { get; set; }

        public int PaymentMethodId { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }

        [NotMapped]
        public bool IsReceiptPosted { get; set; }
    }
}
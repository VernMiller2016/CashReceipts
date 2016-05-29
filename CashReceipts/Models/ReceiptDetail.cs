using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class ReceiptDetail
    {
        public int ReceiptDetailID { get; set; }

        [Display(Name = "Check/Warrant/Other")]
        [Required]
        public string CheckWarrant { get; set; }

        [Display(Name = "Amount")]
        [Required]
        public decimal CheckWarrantAmount { get; set; }

        //public int TenderID { get; set; }
        //public virtual Tender Tenders { get; set; }

        public int ReceiptBodyID { get; set; }

        public virtual ReceiptBody ReceiptBodys { get; set; }

    }
}


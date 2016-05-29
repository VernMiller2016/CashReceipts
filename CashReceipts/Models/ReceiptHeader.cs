using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class ReceiptHeader
    {
        [Key]
        [Display(Name = "Receipt #")]
        public int ReceiptHeaderID { get; set; }

        [Required]
        [Display(Name = "Receipt Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime ReceiptDate { get; set; }

        [Required]
        [Display(Name = "Receipt Total")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public decimal ReceiptTotal { get; set; }

        public virtual IList<ReceiptBody> ReceiptsBody { get; set; }

        public virtual IList<Tender> Tenders { get; set; }

        public int ClerkID { get; set; }

        public virtual Clerk Clerks { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public sealed class ReceiptHeader
    {
        public ReceiptHeader()
        {
            ReceiptBodyRecords = new List<ReceiptBody>();
            Tenders = new List<Tender>();
        }

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

        public int ReceiptNumber { get; set; }

        public IList<ReceiptBody> ReceiptBodyRecords { get; set; }

        public IList<Tender> Tenders { get; set; }

        public int ClerkID { get; set; }

        public Clerk Clerk { get; set; }

        public int DepartmentID { get; set; }

        public Department Department { get; set; }

        [MaxLength(250)]//received from
        public string Comments { get; set; }

        public bool IsDeleted { get; set; }

        [MaxLength(250)]
        public string ReceivedFor { get; set; }

        public bool IsPosted { get; set; }

    }
}

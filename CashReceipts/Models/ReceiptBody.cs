using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class ReceiptBody
    {
        public int ReceiptBodyID { get; set; }

        [Display(Name = "Line Total")]
        [Required]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public decimal LineTotal { get; set; }

        public int ReceiptHeaderID { get; set; }

        public virtual ReceiptHeader ReceiptHeaders { get; set; }

        public int TemplateID { get; set; }

        public virtual Template Template { get; set; }

    }
}

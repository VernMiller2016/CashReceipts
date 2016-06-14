using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class Clerk
    {
        public int ClerkID { get; set; }

        [StringLength(25, ErrorMessage = "First name cannot be longer than 25 characters.")]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [StringLength(25, ErrorMessage = "Last name cannot be longer than 25 characters.")]
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        public virtual ICollection<ReceiptHeader> ReceiptHeaders { get; set; }
    }
}
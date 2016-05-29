using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CashReceipts.Models
{
    public class Entity
    {
        public int EntityID { get; set; }

        [StringLength(40)]
        [Required]
        public string Name { get; set; }

        [StringLength(35)]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [StringLength(35)]
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(2, ErrorMessage = "State cannot be longer than 2 characters.")]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        [StringLength(10, ErrorMessage = "Zip Code cannot be longer than 10 characters.")]
        public string ZipCode { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            ErrorMessage = "Telephone format is not valid.")]
        public string Telephone { get; set; }
    }
}
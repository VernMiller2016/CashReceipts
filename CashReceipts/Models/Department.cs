using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(40, ErrorMessage = "Name cannot be longer than 40 characters.")]
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Template> Templates { get; set; }
    }
}
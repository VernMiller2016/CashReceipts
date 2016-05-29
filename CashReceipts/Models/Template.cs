using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class Template
    {
        public int TemplateID { get; set; }

        [StringLength(3, ErrorMessage = "Fund cannot be longer than 3 characters.")]
        [Required]
        public string Fund { get; set; }

        [StringLength(3, ErrorMessage = "Dept cannot be longer than 3 characters.")]
        [Required]
        public string Dept { get; set; }

        [Display(Name = "Prog")]
        [StringLength(2, ErrorMessage = "Program cannot be longer than 2 characters.")]
        public string Program { get; set; }

        [Display(Name = "Proj")]
        [StringLength(4, ErrorMessage = "Project cannot be longer than 4 characters.")]
        public string Project { get; set; }

        [Display(Name = "Base/Elem/Obj/Dtl")]
        [StringLength(9, ErrorMessage = "Base/Element/Object/Detail cannot be longer than 9 characters.")]
        public string BaseElementObjectDetail { get; set; }

        [StringLength(35, ErrorMessage = "Description cannot be longer than 35 characters.")]
        [Required]
        public string Description { get; set; }

        public int DepartmentID { get; set; }

        public virtual Department Departments { get; set; }
    }
}
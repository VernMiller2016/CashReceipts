using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashReceipts.Models
{
    public class RoleFeaturePermission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public string RoleId { get; set; }

        public int FeatureId { get; set; }

        public IdentityRole Role { get; set; }

        public ScreenFeature Feature { get; set; }
    }
}
using System.Collections.Generic;

namespace CashReceipts.Models
{
    public class ScreenFeature
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ScreenId { get; set; }

        public Screen Screen { get; set; }

        public ICollection<RoleFeaturePermission> Roles { get; set; }
    }
}
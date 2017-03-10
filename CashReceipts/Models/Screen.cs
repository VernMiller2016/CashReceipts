using System.Collections.Generic;

namespace CashReceipts.Models
{
    public class Screen
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ScreenFeature> Features { get; set; }
    }
}
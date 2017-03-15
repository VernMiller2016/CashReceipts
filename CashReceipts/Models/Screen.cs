using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashReceipts.Models
{
    public class Screen
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ScreenFeature> Features { get; set; }
    }
}
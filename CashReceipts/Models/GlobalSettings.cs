using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class GlobalSetting
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
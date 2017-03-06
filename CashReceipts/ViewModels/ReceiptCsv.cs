namespace CashReceipts.ViewModels
{
    public class ReceiptCsv
    {
        public int ReceiptNumber { get; set; }
        public string ReceiptDate { get; set; }
        public string Department { get; set; }
        public string Clerk { get; set; }
        public decimal ReceiptTotal { get; set; }
        public string ReceivedFrom { get; set; }
        public string ReceivedFor { get; set; }
    }
}
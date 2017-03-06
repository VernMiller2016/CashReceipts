namespace CashReceipts.ViewModels
{
    public class ReceiptDetailsCsv
    {
        public int ReceiptNumber { get; set; }
        public string ReceiptDate { get; set; }
        public string Department { get; set; }
        public string Clerk { get; set; }
        public decimal ReceiptTotal { get; set; }
        public string ReceivedFrom { get; set; }
        public string ReceivedFor { get; set; }
        public string AccountNumber { get; set; }
        public decimal LineTotal { get; set; }
        public string Template { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
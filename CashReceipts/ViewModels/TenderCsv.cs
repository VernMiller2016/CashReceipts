namespace CashReceipts.ViewModels
{
    public class TenderCsv
    {
        public int ReceiptNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
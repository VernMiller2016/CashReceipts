namespace CashReceipts.ViewModels
{
    public class LineItemCsv
    {
        public int ReceiptNumber { get; set; }
        public string AccountNumber { get; set; }
        public decimal LineTotal { get; set; }
        public string Template { get; set; }
    }
}
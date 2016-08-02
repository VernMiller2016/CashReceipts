namespace CashReceipts.ViewModels
{
    public class AutoCompleteViewModel
    {
        public string value { get; set; }
        public string field { get; set; }
        public string @operator { get; set; }
        public bool ignoreCase { get; set; }
        public int skip { get; set; }
        public int page { get; set; }
    }
}
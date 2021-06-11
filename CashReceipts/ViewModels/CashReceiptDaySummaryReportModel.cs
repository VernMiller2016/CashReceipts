using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashReceipts.ViewModels
{
    public class CashReceiptDaySummaryReportModel
    {
        public int ReceiptHeaderID { get; set; }
        public int DepartmentId { get; set; }
        public int ReceiptNumber { get; set; }
        public string DepartmentName { get; set; }
        public string Locked { get; set; }
        public string Date { get; set; }
        public decimal Total { get; set; }
        public decimal VirtualTotal { get; set; }
        public string Void { get; set; }
        public string InTotal { get; set; }
    }
}
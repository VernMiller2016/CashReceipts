using System;
using CashReceipts.Models;

namespace CashReceipts.ViewModels
{
    public class AuditVm
    {
        public int Id { get; set; }
        public DateTime ActionDate { get; set; }
        public string UserId { get; set; }
        public SysEntityType EntityType { get; set; }
        public OperationType OperationType { get; set; }
        public string EntityId { get; set; }
        public int? ReceiptNumber { get; set; }
    }
}
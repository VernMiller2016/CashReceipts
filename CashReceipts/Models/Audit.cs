using System;
using System.ComponentModel.DataAnnotations;

namespace CashReceipts.Models
{
    public class Audit
    {
        public int Id { get; set; }

        [Required]
        public OperationType OperationType { get; set; }

        [Required]
        public SysEntityType EntityType { get; set; }

        [Required]
        public string EntityId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }
    }
}
using CashReceipts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashReceipts.Helpers
{
    public class LookupHelper
    {
        private readonly ApplicationDbContext _context;

        public LookupHelper(ApplicationDbContext context)
        {
            _context = context;
        }


        public string LastReceiptId
        {
            get
            {
                var lastReceipt = _context.GlobalSettings.SingleOrDefault(x => x.Key == ConstStrings._lastReceiptId);
                return lastReceipt != null ? lastReceipt.Value : "No value in database";
            }
        }
    }
}
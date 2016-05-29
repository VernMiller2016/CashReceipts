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
            get { return _context.GlobalSettings.Single(x => x.Key == ConstStrings._lastReceiptId).Value; }
        }
    }
}
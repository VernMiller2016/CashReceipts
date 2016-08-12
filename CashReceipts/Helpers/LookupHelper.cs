using CashReceipts.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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


        public int LastReceiptId
        {
            get
            {
                var lastReceipt = _context.GlobalSettings.SingleOrDefault(x => x.Key == ConstStrings._lastReceiptId);
                return lastReceipt != null ? int.Parse(lastReceipt.Value) : 1;
            }
            set
            {
                var lastReceipt = _context.GlobalSettings.SingleOrDefault(x => x.Key == ConstStrings._lastReceiptId);
                if (lastReceipt != null)
                    lastReceipt.Value = value.ToString();
            }
        }

        public bool IsAzure => bool.Parse(ConfigurationManager.AppSettings["IsAzure"] ?? "false");

        public string GcDbName => IsAzure ? "" : "GC.";
        public string DistDbName => IsAzure ? "" : "dist.";

    }
}
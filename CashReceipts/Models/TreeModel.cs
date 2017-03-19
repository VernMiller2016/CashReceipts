using NPOI.Util.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashReceipts.Models
{
    [Serializable]
    public class TreeViewModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }
        public string spriteCssClass { get; set; }
        public bool hasFeatures => items.Count > 0;
        public bool selected => items.Any() && items.All(x => x.selected);
        public List<TreeModel> items { get; set; }
    }

    [Serializable]
    public class TreeModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
        public bool @checked => selected;
        public bool hasFeatures => false;
    }
}
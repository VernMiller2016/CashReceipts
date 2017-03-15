using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashReceipts.Models
{
    [Serializable]
    public class TreeViewModel
    {
        public int Id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }
        public string spriteCssClass { get; set; }
        public List<TreeModel> Items { get; set; }
    }
    [Serializable]
    public class TreeModel
    {
        public int Id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }
        public string spriteCssClass { get; set; }
    }
}
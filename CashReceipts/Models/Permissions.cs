using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CashReceipts.Models
{
    public class Permissions
    {
        public List<string> SelectedFeatures { get; set; }
        public List<string> UnSelectedFeatures { get; set; }
        public List<string> SelectedScreens { get; set; }
        public List<string> UnSelectedScreens { get; set; }

        public Permissions()
        {
            SelectedFeatures = new List<string>();
            UnSelectedFeatures = new List<string>();
            SelectedScreens = new List<string>();
            UnSelectedScreens = new List<string>();

        }
    }
}
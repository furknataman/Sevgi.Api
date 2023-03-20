using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevgi.Model
{
    public class Sale
    {
       
        public int SaleId { get; set; } 
        public string BranchCode { get; set; } = string.Empty;
        public string CashCode { get; set; } = string.Empty;
        public string CashierCode { get; set; } = string.Empty;
        public string CardNo { get; set; } = string.Empty;
        public double Total { get; set; } 
        public DateTime AddDate { get; set; } 
        public bool IsReturn { get; set; }
        public double CardTotal { get; set; }



    }
}

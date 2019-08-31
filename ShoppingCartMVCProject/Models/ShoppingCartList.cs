using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartMVCProject.Models
{
    public class ShoppingCartList
    {
        public int PID { get; set; }
        public int Qty { get; set; }
        public decimal TotAmt { get; set; }
    }
}
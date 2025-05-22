using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ApiControllers
{
    public class BillDTO
    {
        public int UserID { get; set; }
        public int BillAmt { get; set; }
        public List<BillDetailDTO> Details { get; set; }
    }

    public class BillDetailDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
        public string ProductDescription { get; set; }

    }

}
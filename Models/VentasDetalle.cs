using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestCoreMVC.Models
{
    public partial class VentasDetalle
    {
        public int IdVent { get; set; }

        public int? IdUser { get; set; }

        public int? IdProd { get; set; }

        public decimal? PrecioVent { get; set; }

        public virtual Producto IdProdNavigation { get; set; }

        public virtual Usuario IdUserNavigation { get; set; }
    }
}

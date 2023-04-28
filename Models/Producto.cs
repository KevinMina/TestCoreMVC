using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestCoreMVC.Models
{
    public partial class Producto
    {
        public int IdProd { get; set; }

        public string NombreProd { get; set; }

        public decimal? PrecioProd { get; set; }

        public bool? EstadoProd { get; set; }

        public virtual ICollection<VentasDetalle> VentasDetalles { get; set; } = new List<VentasDetalle>();
    }
}

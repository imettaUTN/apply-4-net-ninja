using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ninja.model.Entity {

    public class InvoiceDetail {

        /// <summary>
        /// Valor del IVA para todo el dominio
        /// </summary>
        public double Taxes { get { return 1.21; } }

        public long InvoiceId { get; set; }
        public long Id { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get { return this.Amount * this.UnitPrice; } }
        //Corrigo la propiedad para que calcule bien el precio con impuestos.
        public double TotalPriceWithTaxes { get { return this.TotalPrice * this.Taxes; } }

    }

}
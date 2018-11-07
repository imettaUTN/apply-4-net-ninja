using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ninja.model.Entity {

    public class Invoice {

        public Invoice() {

            this.Detail = new List<InvoiceDetail>();

        }

        public enum Types {
            A,
            B,
            C
        }

        /// <summary>
        /// Numero de factura
        /// </summary>
      
        public long Id { get; set; }
        public string Type { get; set; }
        private IList<InvoiceDetail> Detail { get; set; }

        public IList<InvoiceDetail> GetDetail() {

            return this.Detail;

        }
        public InvoiceDetail  GetDetail(long xid)
        {

            return this.Detail.Where(x => x.Id == xid).FirstOrDefault();

        }

        public void AddDetail(InvoiceDetail detail) {

            this.Detail.Add(detail);

        }

        public void DeleteDetails() {

            this.Detail.Clear();

        }
        /// <summary>
        /// Borra un detalle de la factura
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDetail(long xDetailID)
        {
            //Valido que exista el detalle a eliminar
            if (!ExistsDetail(xDetailID)) {
                throw new Exception("No existe Factura");
            }
            InvoiceDetail invDetail = this.GetDetail().Where(x => x.Id == xDetailID).FirstOrDefault();

            this.Detail.Remove(invDetail);

        }
        /// <summary>
        /// Evalua si existe el detallle de una factura.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsDetail(long xDetailID) {
            InvoiceDetail invDetail = this.GetDetail().Where(x => x.Id == xDetailID).FirstOrDefault();

            return this.GetDetail().IndexOf(invDetail) >-1;
        }
        /// <summary>
        /// Sumar el TotalPrice de cada elemento del detalle
        /// </summary>
        /// <returns></returns>
        public double CalculateInvoiceTotalPriceWithTaxes() {

            double sum = 0;

            foreach(InvoiceDetail item in this.Detail)
                sum += item.TotalPriceWithTaxes;

            return sum;

        }
        public double CalculateInvoiceTotalPriceWithOutTaxes()
        {

            double sum = 0;

            foreach (InvoiceDetail item in this.Detail)
                sum += item.TotalPrice;

            return sum;

        }
    }

}
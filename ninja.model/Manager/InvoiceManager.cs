using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ninja.model.Entity;
using ninja.model.Mock;

namespace ninja.model.Manager {

    public class InvoiceManager {

        private InvoiceMock _mock;

        public InvoiceManager() {

            this._mock = InvoiceMock.GetInstance();

        }
        public IList<Invoice> Facturas { get; set; }

        public IList<Invoice> GetAll() {

            return this._mock.GetAll();

        }

        public Invoice GetById(long id) {

            return this._mock.GetById(id);

        }

        public void Insert(Invoice item) {

            this._mock.Insert(item);

        }

        public void Delete(long id) {

            Invoice invoice = this.GetById(id);
            this._mock.Delete(invoice);

        }

        public Boolean Exists(long id) {

            return this._mock.Exists(id);

        }

        public void UpdateDetail(long id, IList<InvoiceDetail> detail) {

            /*
              Este método tiene que reemplazar todos los items del detalle de la factura
              por los recibidos por parámetro
             */

            #region Escribir el código dentro de este bloque
            if (!this._mock.Exists(id)) { throw new Exception("No existe Factura"); }
            //Busco la factura por el id dado por parametro en el mock
            Invoice Factura = this._mock.GetById(id);
            /*Por cada Detalle de factura recibido:
             * Borro el detalle de la factura s/id
             * Agrego el detalle de la factura actualizado.
            */
            foreach(InvoiceDetail sinvD in detail){
                if (Factura.ExistsDetail(sinvD.Id))
                {
                    Factura.DeleteDetail(sinvD.Id);
                }
            // Se agrega a la factura si no existe.
                Factura.AddDetail(sinvD);                
            }


            #endregion Escribir el código dentro de este bloque

        }

    }

}
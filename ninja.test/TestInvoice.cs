using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ninja.model.Entity;
using ninja.model.Manager;
using ninja.model.Mock;

namespace ninja.test {

    [TestClass]
    public class TestInvoice {

        [TestMethod]
        public void InsertNewInvoice() {

            InvoiceManager manager = new InvoiceManager();
            long id = 1006;
            Invoice invoice = new Invoice() {
                Id = id,
                Type = Invoice.Types.A.ToString()
            };

            manager.Insert(invoice);
            Invoice result = manager.GetById(id);

            Assert.AreEqual(invoice, result);

        }

        [TestMethod]
        public void InsertNewDetailInvoice() {

            InvoiceManager manager = new InvoiceManager();
            long id = 1006;
            Invoice invoice = new Invoice() {
                Id = id,
                Type = Invoice.Types.A.ToString()
            };

            invoice.AddDetail(new InvoiceDetail() {
                Id = id,
                InvoiceId = id,
                Description = "Venta insumos varios",
                Amount = 14,
                UnitPrice = 4.33
            });

            invoice.AddDetail(new InvoiceDetail() {
                Id = id,
                InvoiceId = 6,
                Description = "Venta insumos tóner",
                Amount = 5,
                UnitPrice = 87
            });

            manager.Insert(invoice);
            Invoice result = manager.GetById(id);

            Assert.AreEqual(invoice, result);

        }

        [TestMethod]
        public void DeleteInvoice() {

            /*
              1- Eliminar la factura con id=4
              2- Comprobar de que la factura con id=4 ya no exista
              3- La prueba tiene que mostrarse que se ejecuto correctamente
            */

            #region Escribir el código dentro de este bloque
            //Pongo en un solo lugar el id a borrar, para hacer reutilizable el metodo con distintos ids.
            long sIdFacturaABorrar = 4;
            InvoiceMock sManager = InvoiceMock.GetInstance();
            //Valido que la factura con id 4 exista,Sino existe que tire excepcion.
            if (!sManager.Exists(sIdFacturaABorrar))
            {
               Assert.Fail("La Factura id=4 no existe");
            }

            //Busco la factura por id 
            Invoice sFacturaABorrar = sManager.GetById(sIdFacturaABorrar);

            //Borro la factura
            sManager.Delete(sFacturaABorrar);
            //Valido que se haya borrado la factura.
            if (sManager.Exists(4))
            {
                Assert.Fail("La Factura id=4 no se ha borrado correctamente");
            }

            Assert.AreEqual(sFacturaABorrar.Id, sIdFacturaABorrar);

            #endregion Escribir el código dentro de este bloque

        }

        [TestMethod]
        public void UpdateInvoiceDetail() {

            long id = 1003;
            InvoiceManager manager = new InvoiceManager();
            IList<InvoiceDetail> detail = new List<InvoiceDetail>();

            detail.Add(new InvoiceDetail() {
                Id = 1,
                InvoiceId = id,
                Description = "Venta insumos varios",
                Amount = 14,
                UnitPrice = 4.33
            });

            detail.Add(new InvoiceDetail() {
                Id = 2,
                InvoiceId = id,
                Description = "Venta insumos tóner",
                Amount = 5,
                UnitPrice = 87
            });

            manager.UpdateDetail(id, detail);
            Invoice result = manager.GetById(id);

            Assert.AreEqual(2, result.GetDetail().Count());

        }

        [TestMethod]
        public void CalculateInvoiceTotalPriceWithTaxes() {
            //se cambia el id de la factura por uno existente.
            long id = 1003;
            InvoiceManager manager = new InvoiceManager();
            if (!manager.Exists(id)) {
                throw new Exception("La factura con id =" + id + " no existe");
            }
            Invoice invoice = manager.GetById(id);

            double sum = 0;
            foreach(InvoiceDetail item in invoice.GetDetail()) 
                sum += item.TotalPrice * item.Taxes;

            Assert.AreEqual(sum, invoice.CalculateInvoiceTotalPriceWithTaxes());

        }

    }

}
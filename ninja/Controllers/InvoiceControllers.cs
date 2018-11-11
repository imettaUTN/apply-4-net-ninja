using ninja.model.Entity;
using ninja.model.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ninja.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: InvoiceController
        public ActionResult Index()
        {
            return View(ConverToInvoiceModelView(facturas));
        }

        // GET: InvoiceController/Details/5
        public ActionResult Details(int id)
        {

            return View(ConverToInvoiceModelView(facturas.GetById(id)));
        }
        // GET: InvoiceController/Create
        public ActionResult Create()
        {
            return View(new Models.InvoiceViewModels());
        }

        // POST: InvoiceController/Create
        [HttpPost]
        public ActionResult Create(Models.InvoiceViewModels model, string action)
        {
            try
            {

                if (action == "generarFactura")
                {
                    if (model.InvoiceId >= 0)
                    {
                        //Agrego la factura en una variable de sesion para emular el guardado en base de datos
                        if (!facturas.Exists(model.InvoiceId))
                        {
                            facturas.Insert(model.ToModel());
                            ActualizarFacturasEnSesion(facturas);
                        }
                        return Redirect("~Invoice/Index");

                    }
                    else
                    {
                        throw new Exception("Debe agregar un id para el comprobante");
                    }
                }
                else if (action == "agregarItemFactura")
                {
                    model.DetailID += model.InvoiceDetail.Count() + 1;
                    // Si no ha pasado nuestra validación, mostramos el mensaje personalizado de error
                    if (!model.NewInvoiceValid())
                    {
                        throw new Exception("Solo puede agregar un item válido al detalle");
                    }
                    else if (model.ExistsInDetail(model.DetailDescription))
                    {
                        throw new Exception("El item elegido ya existe en el detalle");
                    }
                    else
                    {
                        model.AddItemDetail();
                    }
                }
                else if (action == "retirarFactura")
                {
                    model.DeleteItemFromDetail();
                }
                else
                {
                    throw new Exception("Acción no definida ..");
                }

                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        // GET: InvoiceController/Edit/5
        public ActionResult Edit(long id)
        {
            return View(ConverToInvoiceModelView(facturas.GetById(id)));
        }

        // POST: InvoiceController/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Invoice xFactura)
        {
            try
            {
                // Invoice sFactura = new Invoice();
                facturas.Delete(xFactura.Id);
                facturas.Insert(xFactura);
                ActualizarFacturasEnSesion(facturas);

                return RedirectToAction("Index");
            }
            catch
            {
                return View(ConverToInvoiceModelView(xFactura));
            }
        }

        // GET: InvoiceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(ConverToInvoiceModelView(facturas.GetById(id)));
        }

        [HttpPost]
        public ActionResult Delete(Models.InvoiceViewModels model, string action)
        {
            try
            {
                if (action.Equals("aceptar"))
                {
                    facturas.Delete(model.InvoiceId);
                    ActualizarFacturasEnSesion(facturas);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

      
        public ActionResult InvoiceDetail(long xInvoiceId, long xDetailID)
        {

            return View(ConverToDetailModelView(facturas.GetById(xInvoiceId).GetDetail(xDetailID)));
        }

        public ActionResult EditDetail(long xInvoiceId, long xDetailID)
        {
            InvoiceDetail sInvDetail = facturas.GetById(xInvoiceId).GetDetail(xDetailID);
            sInvDetail.InvoiceId = xInvoiceId;
            return View(ConverToDetailModelView(sInvDetail));
        }

        // POST: InvoiceController/Edit/5
        [HttpPost]
        public ActionResult EditDetail(long xInvoiceId, long xDetailID, Models.InvoicesDetailViewModel xDetail)
        {
            try
            {
                facturas.GetById(xInvoiceId).DeleteDetail(xDetailID);
                facturas.GetById(xInvoiceId).AddDetail(xDetail.toModel());
                ActualizarFacturasEnSesion(facturas);
                return Redirect("~/Invoice/Edit/" + xInvoiceId);

            }
            catch
            {
                return View(ConverToDetailModelView(xDetail.toModel()));
            }
        }

        public ActionResult DeleteDetail(long xInvoiceId, long xDetailID)
        {
            InvoiceDetail sInvDetail;
            sInvDetail = facturas.GetById(xInvoiceId).GetDetail(xDetailID);
            sInvDetail.InvoiceId = xInvoiceId;
            return View(ConverToDetailModelView(sInvDetail));
        }

        // POST: InvoiceController/Delete/5
        [HttpPost]
        public ActionResult DeleteDetail(long xInvoiceId, long xDetailID, Models.InvoiceViewModels xInvoice)
        {
            facturas.GetById(xInvoiceId).DeleteDetail(xDetailID);
            ActualizarFacturasEnSesion(facturas);
            return Redirect("~/Invoice/Edit/" + xInvoiceId);

        }
        public ActionResult CreateDetail(int xInvoiceId)
        {
            InvoiceDetail sInvDetail = new InvoiceDetail();
            sInvDetail.InvoiceId = xInvoiceId;
            return View(ConverToDetailModelView(sInvDetail));
        }

        // POST: InvoiceController/Delete/5
        [HttpPost]
        public ActionResult CreateDetail(int xInvoiceId, Models.InvoicesDetailViewModel xInvoice)
        {
            xInvoice.InvoiceId += facturas.GetById(xInvoiceId).GetDetail().Count() + 1;
            facturas.GetById(xInvoiceId).GetDetail().Add(xInvoice.toModel());
            ActualizarFacturasEnSesion(facturas);
            return Redirect("~/Invoice/Edit/" + xInvoiceId);

        }
        #region MetodoPublicos

        public InvoiceManager facturas
        {
            get
            {
                InvoiceManager sfacturas = new InvoiceManager();
                if (Session["InvoicesInSession"] != null)
                {
                    InvoiceManager sFacturasAux;
                    sFacturasAux = (InvoiceManager)Session["InvoicesInSession"];
                    return sFacturasAux;
                }
                else { return sfacturas; }
            }
        }

        #endregion
        #region MetodoPrivados

        private void ActualizarFacturasEnSesion(InvoiceManager xFacturas)
        {
            Session["InvoicesInSession"] = xFacturas;
        }

        private Models.InvoiceViewModels ConverToInvoiceModelView(Invoice xFacturas) {
            Models.InvoiceViewModels sInv = new Models.InvoiceViewModels();
            sInv.InvoiceId = xFacturas.Id;
            sInv.Type = xFacturas.Type;
            sInv.InvoiceDetail = ConverToDetailModelView(xFacturas.GetDetail());
            return sInv;
        }

        private IList<Models.InvoiceViewModels> ConverToInvoiceModelView(InvoiceManager xFacturas)
        {
            IList<Models.InvoiceViewModels> sLista = new List<Models.InvoiceViewModels>();
            foreach (Invoice sInvoice in xFacturas.GetAll())
            {
                sLista.Add(ConverToInvoiceModelView(sInvoice));
            }
            return sLista;
        }
        private IList<Models.InvoicesDetailViewModel> ConverToDetailModelView(IList<InvoiceDetail> xInvoiceDetail)
        {
            IList<Models.InvoicesDetailViewModel> sLista = new List<Models.InvoicesDetailViewModel>();
            foreach (InvoiceDetail sInvDetalle in xInvoiceDetail) {
                sLista.Add(ConverToDetailModelView(sInvDetalle));
             }
            return sLista;
        }
        private Models.InvoicesDetailViewModel ConverToDetailModelView(InvoiceDetail xInvoiceDetail) {
            Models.InvoicesDetailViewModel sInvD = new Models.InvoicesDetailViewModel();
            sInvD.Description = xInvoiceDetail.Description;
            sInvD.Amount = xInvoiceDetail.Amount;
            sInvD.DetailID = xInvoiceDetail.Id;
            sInvD.InvoiceId = xInvoiceDetail.InvoiceId;
            sInvD.Taxes = xInvoiceDetail.Taxes;
            sInvD.UnitPrice = xInvoiceDetail.UnitPrice;
            return sInvD;
        }

        #endregion
    }
}

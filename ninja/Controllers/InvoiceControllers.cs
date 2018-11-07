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
            return View(facturas.GetAll());
        }

        // GET: InvoiceController/Details/5
        public ActionResult Details(int id)
        {

            return View(facturas.GetById(id));
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
                    if (model.CabeceraId >= 0)
                    {
                        //Agrego la factura en una variable de sesion para emular el guardado en base de datos
                        if (!facturas.Exists(model.CabeceraId))
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
                    model.DetalleId += model.FacturaDetalle.Count() + 1;
                    // Si no ha pasado nuestra validación, mostramos el mensaje personalizado de error
                    if (!model.SeAgregoUnaFacturaValida())
                    {
                        throw new Exception("Solo puede agregar un item válido al detalle");
                    }
                    else if (model.ExisteEnDetalle(model.descripcionDetalle))
                    {
                        throw new Exception("El item elegido ya existe en el detalle");
                    }
                    else
                    {
                        model.AgregarItemADetalle();
                    }
                }
                else if (action == "retirarFactura")
                {
                    model.RetirarItemDeDetalle();
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
            return View(facturas.GetById(id));
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

                return View(xFactura);
            }
            catch
            {
                return View(xFactura);
            }
        }

        // GET: InvoiceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(facturas.GetById(id));
        }

        // POST: InvoiceController/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Invoice xInvoice)
        {
            try
            {
                facturas.Delete(id);
                ActualizarFacturasEnSesion(facturas);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult InvoiceDetail(long xInvoiceId, long xDetalleId)
        {

            return View(facturas.GetById(xInvoiceId).GetDetail(xDetalleId));
        }

        public ActionResult EditDetail(long xInvoiceId, long xDetalleId)
        {
            InvoiceDetail sInvDetail = facturas.GetById(xInvoiceId).GetDetail(xDetalleId);
            sInvDetail.InvoiceId = xInvoiceId;
            return View(sInvDetail);
        }

        // POST: InvoiceController/Edit/5
        [HttpPost]
        public ActionResult EditDetail(long xInvoiceId, long xDetalleId, InvoiceDetail xDetail)
        {
            try
            {
                facturas.GetById(xInvoiceId).DeleteDetail(xDetalleId);
                facturas.GetById(xInvoiceId).AddDetail(xDetail);
                ActualizarFacturasEnSesion(facturas);
                return Redirect("~/Invoice/Edit/"+xInvoiceId);

            }
            catch
            {
                return View(xDetail);
            }
        }

        public ActionResult DeleteDetail(long xInvoiceId, long xDetalleId)
        {
            InvoiceDetail sInvDetail;
            sInvDetail = facturas.GetById(xInvoiceId).GetDetail(xDetalleId);
            sInvDetail.InvoiceId = xInvoiceId;
            return View(sInvDetail);
        }

        // POST: InvoiceController/Delete/5
        [HttpPost]
        public ActionResult DeleteDetail(long xInvoiceId, long xDetalleId, Invoice xInvoice)
        {
            facturas.GetById(xInvoiceId).DeleteDetail(xDetalleId);
            ActualizarFacturasEnSesion(facturas);
            return Redirect("~/Invoice/Edit/" + xInvoiceId);

        }
        public ActionResult CreateDetail(int xInvoiceId)
        {
            InvoiceDetail sInvDetail = new InvoiceDetail();
            sInvDetail.InvoiceId = xInvoiceId;
            return View(sInvDetail);
        }

        // POST: InvoiceController/Delete/5
        [HttpPost]
        public ActionResult CreateDetail(int xInvoiceId, InvoiceDetail xInvoice)
        {
            xInvoice.Id += facturas.GetById(xInvoiceId).GetDetail().Count() + 1;
            facturas.GetById(xInvoiceId).GetDetail().Add(xInvoice);
            ActualizarFacturasEnSesion(facturas);
            return Redirect("~/Invoice/Edit/" + xInvoiceId);

        }
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
        private void ActualizarFacturasEnSesion(InvoiceManager xFacturas)
        {
            Session["InvoicesInSession"] = xFacturas;
        }
    }
}

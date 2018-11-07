using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ninja.model.Entity;
using System.Linq;

namespace ninja.Models
{
    public class InvoiceViewModels
    {
        #region Cabecera
        public string descripcionDetalle { get; set; }
        public long CabeceraId { get; set; }
        public long DetalleId { get; set; }
        public string CabeceraTipo { get; set; }
        public int CabeceraDetalleCantidad { get; set; }
        public double CabeceraDetallePrecio { get; set; }
        #endregion

        #region Contenido
        public List<InvoicesDetailViewModel> FacturaDetalle { get; set; }
        #endregion

        #region Pie
        public double Total()
        {
            return FacturaDetalle.Sum(x => x.Monto());
        }
        #endregion

        public InvoiceViewModels()
        {
            FacturaDetalle = new List<InvoicesDetailViewModel>();
            Refrescar();
        }

        public void Refrescar()
        {
            CabeceraId = 0;
            DetalleId = 0;
            CabeceraTipo = "";
            CabeceraDetalleCantidad = 1;
            CabeceraDetallePrecio = 0;
        }

        public bool SeAgregoUnaFacturaValida()
        {
            return !(CabeceraId == 0 || DetalleId == 0 || string.IsNullOrEmpty(CabeceraTipo) || CabeceraDetalleCantidad == 0 || CabeceraDetallePrecio == 0);
        }

        public bool ExisteEnDetalle(long xDetalleId)
        {
            return FacturaDetalle.Any(x => x.DetalleId == xDetalleId);
        }
        public bool ExisteEnDetalle(string xDetalleDesc)
        {
            return FacturaDetalle.Any(x => x.descripcion == xDetalleDesc);
        }

        public void RetirarItemDeDetalle()
        {
            if (FacturaDetalle.Count > 0)
            {
                var detalleARetirar = FacturaDetalle.Where(x => x.Retirar)
                                                        .SingleOrDefault();

                FacturaDetalle.Remove(detalleARetirar);
            }
        }

        public void AgregarItemADetalle()
        {
            FacturaDetalle.Add(new InvoicesDetailViewModel
            {
                descripcion = descripcionDetalle,
                InvoiceId = CabeceraId,
                PrecioUnitario = CabeceraDetallePrecio,
                DetalleId = DetalleId,
                Cantidad = CabeceraDetalleCantidad,
            });

            Refrescar();
        }

        public Invoice ToModel()
        {
            var sInvoice = new Invoice();
            sInvoice.Id = this.CabeceraId;
            sInvoice.Type = this.CabeceraTipo;

            foreach (var sInvDetail in FacturaDetalle)
            {
                sInvoice.AddDetail(new InvoiceDetail
                {
                    Id = sInvDetail.DetalleId,
                    InvoiceId = sInvDetail.InvoiceId,
                    UnitPrice = sInvDetail.PrecioUnitario,
                    Amount = sInvDetail.Cantidad
                });
            }

            return sInvoice;
        }
    }
    public partial class InvoicesDetailViewModel
    {
        public string descripcion { get; set; }
        public long InvoiceId { get; set; }
        public long DetalleId { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public bool Retirar { get; set; }
        public double Monto()
        {
            return Cantidad * PrecioUnitario;
        }
    }
}
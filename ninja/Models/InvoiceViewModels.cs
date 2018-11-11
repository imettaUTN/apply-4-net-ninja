using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ninja.model.Entity;
using System.Linq;

namespace ninja.Models
{
    public class InvoiceViewModels
    {
        #region Cabecera
        public string DetailDescription { get; set; }
        public long InvoiceId { get; set; }
        public long DetailID { get; set; }
        public string Type { get; set; }
        public int DetailCount { get; set; }
        public double DetailPrice { get; set; }
        #endregion

        #region Contenido
        public IList<InvoicesDetailViewModel> InvoiceDetail { get; set; }
        #endregion

        #region Pie
        public double Total()
        {
            return InvoiceDetail.Sum(x => x.PriceWithTaxes());
        }
        #endregion

        public InvoiceViewModels()
        {
            InvoiceDetail = new List<InvoicesDetailViewModel>();
            Refrescar();
        }

        public void Refrescar()
        {
            InvoiceId = 0;
            DetailID = 0;
            Type = "";
            DetailCount = 1;
            DetailPrice = 0;
        }

        public bool NewInvoiceValid()
        {
            return !(InvoiceId == 0 || DetailID == 0 || string.IsNullOrEmpty(Type) || DetailCount == 0 || DetailPrice == 0);
        }

        public bool ExistsInDetail(long xDetailID)
        {
            return InvoiceDetail.Any(x => x.DetailID == xDetailID);
        }
        public bool ExistsInDetail(string xDetalleDesc)
        {
            return InvoiceDetail.Any(x => x.Description == xDetalleDesc);
        }

        public void DeleteItemFromDetail()
        {
            if (InvoiceDetail.Count > 0)
            {
                var detalleARemoveItem = InvoiceDetail.Where(x => x.RemoveItem)
                                                        .SingleOrDefault();

                InvoiceDetail.Remove(detalleARemoveItem);
            }
        }

        public void AddItemDetail()
        {
            InvoiceDetail.Add(new InvoicesDetailViewModel
            {
                Description = DetailDescription,
                InvoiceId = InvoiceId,
                UnitPrice = DetailPrice,
                DetailID = DetailID,
                Amount = DetailCount,
            });

            Refrescar();
        }

        public Invoice ToModel()
        {
            var sInvoice = new Invoice();
            sInvoice.Id = this.InvoiceId;
            sInvoice.Type = this.Type;

            foreach (var sInvDetail in InvoiceDetail)
            {
                sInvoice.AddDetail(new InvoiceDetail
                {
                    Id = sInvDetail.DetailID,
                    InvoiceId = sInvDetail.InvoiceId,
                    UnitPrice = sInvDetail.UnitPrice,
                    Amount = sInvDetail.Amount
                });
            }

            return sInvoice;
        }
        public double CalculateInvoiceTotalPriceWithTaxes()
        {

            double sum = 0;

            foreach (InvoicesDetailViewModel item in this.InvoiceDetail)
                sum += item.TotalPriceWithTaxes;

            return sum;

        }
        public double CalculateInvoiceTotalPriceWithOutTaxes()
        {

            double sum = 0;

            foreach (InvoicesDetailViewModel item in this.InvoiceDetail)
                sum += item.TotalPrice;

            return sum;

        }
    }
    public partial class InvoicesDetailViewModel
    {
        public string Description { get; set; }
        public long InvoiceId { get; set; }
        public long DetailID { get; set; }
        public double Amount { get; set; }
        public double UnitPrice { get; set; }
        public double Taxes { get; set; }
        public bool RemoveItem { get; set; }
        public double TotalPrice { get { return PriceWithTaxes(); } }
        public double TotalPriceWithTaxes { get { return PriceWithTaxes(); } }

        public double PriceWithOutTaxes()
        {
            return Amount * UnitPrice;
        }
        public double PriceWithTaxes()
        {
            return Amount * (UnitPrice + Taxes);
        }

       public InvoiceDetail toModel()
        {
            InvoiceDetail sDetailInv = new InvoiceDetail();
            sDetailInv.Id = this.DetailID;
            sDetailInv.InvoiceId = this.InvoiceId;
            sDetailInv.UnitPrice = this.UnitPrice;
            sDetailInv.Amount = this.Amount;
            return sDetailInv;
        }
    }
}
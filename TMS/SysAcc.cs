using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Data;

namespace TMS
{
    public static class SysAcc
    {
        public static TradeFirm GetTF(TMSContext context, int id)
        {
            try
            {
                TradeFirm tf = context.TradeFirms.Where(a => a.Id == id).FirstOrDefault();
                return tf;
            }
            catch
            {
                return null;
            }
        }
        public static TradeFirm GetTF(TMSContext context, string userId)
        {
            try
            {
                User u = context.Users.Where(a => a.Id == userId).FirstOrDefault();
                if(u == null || u.TradeFirmId == null)
                {
                    return null;
                }
                TradeFirm tf = context.TradeFirms.Where(a => a.Id == u.TradeFirmId).FirstOrDefault();
                return tf;
            }
            catch
            {
                return null;
            }
        }

        public static DateTime GetLocalDate()
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime currDate = TimeZoneInfo.ConvertTime(DateTime.Now, tzi);
            return currDate;
        }
        public static DateTime GetCurrentTime(DateTime currDate)
        {
            DateTime currDateTime = new DateTime(currDate.Year, currDate.Month, currDate.Day, GetLocalDate().Hour, GetLocalDate().Minute, GetLocalDate().Second);
            return currDateTime;
        }

        public static double GetCost(TMSContext context, long productId)
        {
            try
            {
                double cost = 0;
                var product = context.Products.Where(a => a.Id == productId).FirstOrDefault();
                if (product != null)
                {
                    cost = product.CostPrice;
                }

                var stockValue = context.ProductLines.Where(a => a.TransactionStatusId != 3 && a.ProductId == productId).Sum(a => a.StockValue);
                var stock = context.ProductLines.Where(a => a.TransactionStatusId != 3 && a.ProductId == productId).Sum(a => a.Stock);
                if (stockValue > 0 && stock > 0)
                {
                    cost = Math.Round(stockValue / stock, 0);
                }
                return cost;
            }
            catch
            {
                return 0;
            }
        }
        public static double GetCost(TMSContext context, long productId, long docId, int docTypeId)
        {
            try
            {
                double cost = 0;
                var product = context.Products.Where(a => a.Id == productId).FirstOrDefault();
                if(product != null)
                {
                    cost = product.CostPrice;
                }

                var stockValue = context.ProductLines.Where(a => a.TransactionStatusId != 3 && a.ProductId == productId && !(a.DocId == docId && a.DocTypeId == docTypeId)).Sum(a => a.StockValue);
                var stock = context.ProductLines.Where(a => a.TransactionStatusId != 3 && a.ProductId == productId && !(a.DocId == docId && a.DocTypeId == docTypeId)).Sum(a => a.Stock);
                if(stockValue > 0 && stock > 0)
                {
                    cost = Math.Round(stockValue / stock, 0);
                }
                return cost;
            }
            catch
            {
                return 0;
            }
        }
    }
}

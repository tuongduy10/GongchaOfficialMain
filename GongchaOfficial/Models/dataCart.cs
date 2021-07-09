using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GongchaOfficial.Models;

namespace GongchaOfficial.Models
{
    public class dataCart
    {
        dbGongchaDataContext db = new dbGongchaDataContext();
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public double ProductPrice { get; set; }
        public string ProductSize { get; set; }
        public int Amount { get; set; }
        public double TotalPrice
        {
            get { return ProductPrice * Amount; }
        }

        public dataCart(string id)
        {
            ProductId = id;
            Product pd = db.Products.Single(p => p.ProductId == ProductId);
            ProductName = pd.ProductName;
            ProductImg = pd.ProductImage;
            ProductPrice = double.Parse(pd.ProductPrice.ToString());
            ProductSize = pd.ProductSize;
            Amount = 1;//Số lượng của sản phẩm mặc định trong giỏ hàng
        }

    }
}
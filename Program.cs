using System;
using System.Collections.Generic;

namespace ProductApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Order1
            ShoppingBasket basket1 = new ShoppingBasket();
            basket1.products.Add(new Product("book", 1, 12.49m, true, false));
            basket1.products.Add(new Product("music CD", 1, 14.99m, false, false));
            basket1.products.Add(new Product("chocolate bar", 1, 0.85m, true, false));
            WriteData(basket1);

            //Order2
            ShoppingBasket basket2 = new ShoppingBasket();
            basket2.products.Add(new Product("imported box of chocolates", 1, 10m, true, true));
            basket2.products.Add(new Product("imported bottle of perfume", 1, 47.5m, false, true));
            WriteData(basket2);

            //Order3
            ShoppingBasket basket3 = new ShoppingBasket();
            basket3.products.Add(new Product("imported bottle of perfume", 1, 27.99m, false, true));
            basket3.products.Add(new Product("bottle of perfume", 1, 18.99m, false, false));
            basket3.products.Add(new Product("packet of headache pills", 1, 9.75m, true, false));
            basket3.products.Add(new Product("box of imported chocolates", 1, 11.25m, true, true));
            WriteData(basket3);

            Console.ReadKey();
        }
        public static void WriteData(ShoppingBasket basket)
        {
            basket.products.ForEach((item) =>
            {
                item.Compute();
                basket.totalSelfPrice += item.selfPrice * item.num;
                basket.totalSaleTax += item.salesTax * item.num;
                Console.WriteLine($"{item.num} {item.name}:{item.selfPrice}");
            });
            Console.WriteLine($"Sales Taxes:{basket.totalSaleTax}");
            Console.WriteLine($"Total:{basket.totalSelfPrice}");
            Console.WriteLine();
        }
    }
    /// <summary>
    /// 购物篮
    /// </summary>
    public class ShoppingBasket
    {
        public List<Product> products { set; get; }
        public decimal totalSelfPrice { set; get; }
        public decimal totalSaleTax { set; get; }
        public ShoppingBasket()
        {
            this.products = new List<Product>();
            this.totalSaleTax = 0;
            this.totalSaleTax = 0;
        }
    }
    /// <summary>
    /// 商品
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { set; get; }
        /// <summary>
        /// 数量
        /// </summary>
        public int num { set; get; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { set; get; }
        /// <summary>
        /// 是否免税;true:免税;false:非免税
        /// </summary>
        public bool enExcempt { set; get; }
        /// <summary>
        /// 是否进口：true->进口；false:非进口
        /// </summary>
        public bool enImport { set; get; }
        /// <summary>
        /// 上架价格
        /// </summary>
        public decimal selfPrice { set; get; }
        /// <summary>
        /// 销售税
        /// </summary>
        public decimal salesTax { set; get; }
        public Product() { }
        public Product(string name, int num, decimal price, bool enExcempt, bool enImport)
        {
            this.name = name;
            this.price = price;
            this.num = num;
            this.enExcempt = enExcempt;
            this.enImport = enImport;
        }
        /// <summary>
        /// 计算策略
        /// </summary>
        private IComputeStrategy computeStrategy { set; get; }
        /// <summary>
        /// 计算
        /// </summary>
        public void Compute()
        {
            this.computeStrategy = ComputeStrategyFactory.GetComputeStrategy(this);
            if (this.computeStrategy == null)
            {
                this.salesTax = 0;
            }
            else
            {
                decimal saleTax = this.computeStrategy.calculateSalesTax(this.price);
                this.salesTax = Math.Ceiling(saleTax / 0.05m) * 0.05m;
            }
            this.selfPrice = this.salesTax + this.price;
        }
    }
    /// <summary>
    /// 计算策略
    /// </summary>
    public interface IComputeStrategy
    {
        decimal calculateSalesTax(decimal price);
    }
    /// <summary>
    /// 非进口免税计算
    /// </summary>
    public class ComputeWithNo : IComputeStrategy
    {
        public decimal calculateSalesTax(decimal price)
        {
            return 0;
        }
    }
    /// <summary>
    /// 进口免税计算
    /// </summary>
    public class ComputeWithImport : IComputeStrategy
    {
        public decimal calculateSalesTax(decimal price)
        {
            decimal calcutePrice = ((price * DiscountRule.importDiscountRate) / 100);
            return calcutePrice;
        }
    }
    /// <summary>
    /// 非免税且非进口计算
    /// </summary>
    public class ComputeWithExcempt : IComputeStrategy
    {
        public decimal calculateSalesTax(decimal price)
        {
            decimal calcutePrice = ((price * DiscountRule.unExcemptDiscountRate) / 100);
            return calcutePrice;
        }
    }
    /// <summary>
    /// 进口非免税计算
    /// </summary>
    public class ComputeWithImportAndExcempt : IComputeStrategy
    {
        public decimal calculateSalesTax(decimal price)
        {
            decimal calcutePrice = ((price * (DiscountRule.unExcemptDiscountRate + DiscountRule.importDiscountRate)) / 100);
            return calcutePrice;
        }
    }
    public class ComputeStrategyFactory
    {
        public static IComputeStrategy GetComputeStrategy(Product product)
        {
            //免税且非进口计算
            if (product.enExcempt && !product.enImport)
            {
                return new ComputeWithNo();
            }
            //免税且进口计算
            if (product.enExcempt && product.enImport)
            {
                return new ComputeWithImport();
            }
            //非免税且非进口
            if (!product.enExcempt && !product.enImport)
            {
                return new ComputeWithExcempt();
            }
            //非免税且进口
            if (!product.enExcempt && product.enImport)
            {
                return new ComputeWithImportAndExcempt();
            }
            return null;
        }
    }
    /// <summary>
    /// 税率规则
    /// </summary>
    public class DiscountRule
    {
        public const decimal importDiscountRate = 5;
        public const decimal unExcemptDiscountRate = 10;
    }
}

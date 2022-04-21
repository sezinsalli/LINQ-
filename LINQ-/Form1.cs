using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LINQ_
{
    public partial class Form1 : Form
    {
        NORTHWNDEntities db = new NORTHWNDEntities();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Fiyatı 20 ile 50 arasında olan ürünlerin ID, ürün adi, fiyatı, stok miktarı ve kategorisi nedir?

            dataGridView1.DataSource = db.Products.Select(x => new { x.ProductID, x.ProductName, x.UnitPrice, x.QuantityPerUnit, x.Categories.CategoryName }).Where(x => x.UnitPrice > 20 && x.UnitPrice < 50).ToList();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Siparişlerin müşteri şirket adı, çalışan adı-soyadı, sipariş tarihi, kargo şirketinin adı ile listeleyiniz.

            dataGridView1.DataSource = (from o in db.Orders
                                        join c in db.Customers
                                        on o.CustomerID equals c.CustomerID
                                        join emp in db.Employees on o.EmployeeID
                                        equals emp.EmployeeID
                                        join s in db.Shippers
                                        on o.ShipVia equals s.ShipperID
                                        select new
                                        {
                                            SIRKETADI = c.CompanyName,
                                            Calısanadısoyadı = emp.FirstName + " " + emp.LastName,
                                            Sıparıstarıhı = o.OrderDate,
                                            kargosırketınınadı = s.CompanyName
                                        }).ToList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Kategorilerine göre stoktaki ürünleri adeti kaçtır?
            dataGridView1.DataSource = (from cat in db.Categories
                                        join pro in db.Products
                                        on cat.CategoryID equals pro.CategoryID
                                        group new { cat, pro } by new { cat.CategoryName } into a
                                        select new
                                        {
                                            categorı = a.Key.CategoryName,
                                            adet = a.Sum(x => x.pro.UnitsInStock)
                                        }).ToList();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Çalışanların ad, soyad, doğum tarihi ve yaş bilgilerini listeleyiniz. Yaşa göre büyükten küçüğe sıralayınız
            dataGridView1.DataSource = db.Employees.Select(x => new { x.FirstName, x.LastName, x.BirthDate, }).OrderBy(x => x.BirthDate).ToList();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Kargo şirketlerinden adi SpeedyExpress olan şirketin tüm bilgilerini listeleyiniz.
            dataGridView1.DataSource = db.Shippers.Select(x => new { x.CompanyName, x.ShipperID, x.Phone, }).Where(x => x.CompanyName == "Speedy Express").ToList();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Stokta bulunan toplam ürün sayısını getiriniz.
            dataGridView1.DataSource = (from p in db.Products
                                        group new { p } by new { p.ProductName } into a
                                        select new
                                        {
                                            urunadı = a.Key.ProductName,
                                            toplam = a.Sum(x => x.p.UnitsInStock)
                                        }).OrderByDescending(x => x.toplam).ToList();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Hangi çalışanım hangi çalışanıma bağlı?
            dataGridView1.DataSource = (from e1 in db.Employees

                                        join e2 in db.Employees
                                        on e1.EmployeeID equals e2.ReportsTo
                                        select new
                                        {
                                            CalisanAdSoyad = e2.FirstName + " " + e1.LastName,
                                            RaporVerenİLENAdSoyad = e1.FirstName + " " + e1.LastName
                                        }).ToList();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Ürünlere göre satışım nasıl?
            dataGridView1.DataSource = (from p in db.Products
                                        join od in db.Order_Details
                                        on p.ProductID equals od.ProductID
                                        group new { p, od } by new { p.ProductName } into a
                                        select new
                                        {
                                            teadıkcı = a.Key.ProductName,
                                            teadıkcıadı = a.Select(x => x.p.ProductName).FirstOrDefault(),
                                            toplam = a.Sum(x => x.od.Quantity)
                                        }).ToList();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Ürün kategorilerine göre satışlarım nasıl? (para bazında)
            dataGridView1.DataSource = (from p in db.Products
                                        join od in db.Order_Details
                                        on p.ProductID equals od.ProductID
                                        join c in db.Categories on p.CategoryID equals c.CategoryID
                                        group new { p, od, c } by new { c.CategoryName } into a
                                        select new
                                        {
                                            Kategorı = a.Key.CategoryName,
                                            Product = a.Select(x => x.p.ProductName).FirstOrDefault(),

                                            toplam = a.Sum(x => x.od.Quantity * x.od.UnitPrice)
                                        }).ToList();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Çalışanlar ürün bazında ne kadarlık satış yapmışlar?
            dataGridView1.DataSource = (from o in db.Orders
                                        join emp in db.Employees
                                        on o.EmployeeID equals emp.EmployeeID
                                        join od in db.Order_Details
                                        on o.OrderID equals od.OrderID
                                        join pr in db.Products on od.ProductID
                                        equals pr.ProductID
                                        group new { o, emp, od, pr }
                                        by new { emp.FirstName } into a
                                        select new
                                        {
                                            calısan = a.Key.FirstName,
                                            calısansoyad = a.Select(x => x.emp.LastName).FirstOrDefault(),
                                            urun = a.Select(x => x.pr.ProductName).FirstOrDefault(),
                                            toplam = a.Sum(x => x.od.Quantity)

                                        }).ToList();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Çalışanlarım para olarak en fazla hangi ürünü satmışlar? Kişi bazında bir rapor istiyorum.
            dataGridView1.DataSource = (from o in db.Orders
                                        join emp in db.Employees
                                        on o.EmployeeID equals emp.EmployeeID
                                        join od in db.Order_Details
                                        on o.OrderID equals od.OrderID
                                        join pr in db.Products on od.ProductID
                                        equals pr.ProductID
                                        group new { o, emp, od, pr }
                                        by new { emp.FirstName } into a
                                        select new
                                        {
                                            calısan = a.Key.FirstName,
                                            calısansoyad = a.Select(x => x.emp.LastName).FirstOrDefault(),
                                            urun = a.Select(x => x.pr.ProductName).FirstOrDefault(),
                                            toplamfıyat = a.Sum(x => x.od.Quantity * x.od.UnitPrice)

                                        }).OrderBy(x => x.toplamfıyat).ToList();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //Hangi kargo şirketine toplam ne kadar ödeme yapmışım
            dataGridView1.DataSource = (from Shipper in db.Shippers
                                        join Order in db.Orders on Shipper.ShipperID equals Order.ShipVia
                                        select new
                                        {
                                            KargocuAdı = Shipper.CompanyName,
                                            Odeme = (decimal)Order.Freight
                                        }).ToList();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //Tost yapmayı seven çalışanım hangisi?
            dataGridView1.DataSource = db.Employees.Where(x => x.Notes.Contains("toast")).ToList();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            //Hangi tedarikçiden aldığım ürünlerden ne kadar satmışım
            dataGridView1.DataSource = (from p in db.Products
                                        join od in db.Order_Details
                                        on p.ProductID equals od.ProductID
                                        join s in db.Suppliers
                                        on p.SupplierID equals s.SupplierID
                                        group new { p, od, s } by new { s.CompanyName } into a
                                        select new
                                        {
                                            teadıkcı = a.Key.CompanyName,
                                            Urun = a.Select(x => x.p.ProductName).FirstOrDefault(),
                                            toplam = a.Sum(x => x.od.Quantity)
                                        }).ToList();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //En değerli müşterim hangisi? (en fazla satış yaptığım müşteri)
            dataGridView1.DataSource = (from o2 in db.Orders
                                        join c2 in db.Customers
                                        on o2.CustomerID equals c2.CustomerID
                                        join od in db.Order_Details
                                        on o2.OrderID equals od.OrderID
                                        group new { o2, c2, od }
                                          by new { c2.CompanyName } into a
                                        select new
                                        {
                                            Musteri = a.Key.CompanyName,


                                            SatısFıyatı = a.Sum(x => x.od.Quantity * x.od.UnitPrice)

                                        }).OrderByDescending(x => x.SatısFıyatı).Take(1).ToList();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            // Hangi müşteriler para bazında en fazla hangi ürünü almışlar?
            dataGridView1.DataSource = (from o2 in db.Orders
                                        join c2 in db.Customers
                                        on o2.CustomerID equals c2.CustomerID
                                        join od in db.Order_Details
                                        on o2.OrderID equals od.OrderID
                                        join p in db.Products on od.ProductID equals p.ProductID
                                        group new { o2, c2, od, p }
                                          by new { c2.CompanyName } into a
                                        select new
                                        {
                                            Musteri = a.Key.CompanyName,

                                            Urun = a.Select(x => x.p.ProductName).FirstOrDefault(),
                                            SatısFıyatı = a.Sum(x => x.od.Quantity * x.od.UnitPrice)

                                        }).OrderByDescending(x => x.SatısFıyatı).ToList();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //Hangi ülkelere ne kadarlık satış yapmışım
            dataGridView1.DataSource = (from o in db.Orders
                                        join od in db.Order_Details
                    on o.OrderID equals od.OrderID
                                        group new { o, od }
         by new { o.ShipCountry } into a
                                        select new
                                        {
                                            Ulke = a.Key.ShipCountry,
                                            UrunMıktarı = a.Sum(x => x.od.Quantity),
                                            SatısFıyatı = a.Sum(x => x.od.Quantity * x.od.UnitPrice)
                                        }).ToList();
        }
    }
}

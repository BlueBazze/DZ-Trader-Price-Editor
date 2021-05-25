using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TestingTraderPrices
{
    public class Trader
    {

        public Trader() { Categories.Add(new Category()); }

        public Trader(string name)
        {
            Name = name;
        }

        public Trader newTrader(string name)
        {
            Name = name;
            return this;
        }

        

        public void newCategory(string categoryName)
        {
            Categories.Add(new Category(categoryName));
        }

        public string Name
        {
            get;
            set;
        }

        public ObservableCollection<Category> Categories = new ObservableCollection<Category>();
    }

    public class Category
    {
        public string Name
        {
            get;
            set;
        }
        public ObservableCollection<Item> items = new ObservableCollection<Item>();

        public Category() { items.Add(new Item()); }
        public Category(string categoryName)
        {
            Name = categoryName;
        }

    }

    public class Item
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
        public int BuyValue { get; set; }
        public int SellValue { get; set; }
        public bool Enabled { get; set; }
        public string Comment { get; set; }

        public Item() { }
        public Item(string name, string quantity, int buyValue, int sellValue, bool enabled, string comment)
        {
            Name = name;
            Quantity = quantity;
            BuyValue = buyValue;
            SellValue = sellValue;
            Enabled = enabled;
            Comment = comment;
        }

    }
}

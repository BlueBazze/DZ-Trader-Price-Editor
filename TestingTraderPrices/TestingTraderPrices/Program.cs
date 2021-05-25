using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TestingTraderPrices
{
    class Program
    {

        public static TraderTable traderlist;

        static void Main(string[] args)
        {
            traderlist = new TraderTable("TraderConfig.txt");
            traderlist.LoadFile();

            for (int trad = 0; trad < traderlist.Traders.Count; trad++)
            {
                for(int cate = 0; cate < traderlist.Traders[trad].Categories.Count; cate++)
                {
                    for (int item = 0; item < traderlist.Traders[trad].Categories[cate].items.Count; item++)
                    {
                        CheckItem(trad, cate, item, "MSFC", "NVG", 5000, 3000);
                        CheckItem(trad, cate, item, "MSFC", "Pants", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "Jacket", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "M65", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "Gloves", 450, 225);
                        CheckItem(trad, cate, item, "MSFC", "Sweater", 300, 125);
                        CheckItem(trad, cate, item, "MSFC", "Boots", 500, 300);

                        CheckItem(trad, cate, item, "MSFC", "Vest", 350, 150);
                        CheckItem(trad, cate, item, "MSFC", "PlateCarrierVest", 2000, 400);
                        CheckItem(trad, cate, item, "MSFC", "Tortila", 400, 150);
                        CheckItem(trad, cate, item, "MSFC", "AliceBag", 1000, 500);
                        CheckItem(trad, cate, item, "MSFC", "NVG", 5000, 3000);
                        CheckItem(trad, cate, item, "MSFC", "JPS_Vest", 800, 250);
                        CheckItem(trad, cate, item, "MSFC", "TEC_Vest", 800, 250);
                        CheckItem(trad, cate, item, "MSFC", "AssaultBag", 1000, 500);
                        CheckItem(trad, cate, item, "MSFC", "Cap", 300, 100);
                        CheckItem(trad, cate, item, "MSFC", "WarBelt", 1500, 450);
                        CheckItem(trad, cate, item, "MSFC", "Defender_Vest", 2000, 900);
                        CheckItem(trad, cate, item, "MSFC", "GorkaJ", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "GorkaP", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "UKVest", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "TShirt", 300, 125);
                        CheckItem(trad, cate, item, "MSFC", "TacVest", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "CoyoteBag", 300, 125);
                        CheckItem(trad, cate, item, "MSFC", "Mich2001", 400, 200);
                        CheckItem(trad, cate, item, "MSFC", "Hood", 400, 200);
                        CheckItem(trad, cate, item, "MSFC", "Bandana", 300, 125);
                        CheckItem(trad, cate, item, "MSFC", "NBC", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "MountainBag", 1000, 500);
                        CheckItem(trad, cate, item, "MSFC", "Holster", 300, 125);
                        CheckItem(trad, cate, item, "MSFC", "Puches", 300, 125);
                        CheckItem(trad, cate, item, "MSFC", "GorkaHelmet", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "Mich2000Helmet", 450, 285);
                        CheckItem(trad, cate, item, "MSFC", "PressVest", 300, 125);
                        CheckItem(trad, cate, item, "MSFC", "SmershVest", 400, 200);
                        CheckItem(trad, cate, item, "MSFC", "TacticalMask", 500, 300);
                        CheckItem(trad, cate, item, "MSFC", "SmershBagMedic", 400, 200);
                        CheckItem(trad, cate, item, "MSFC", "Backpack", 1000, 500);
                        CheckItem(trad, cate, item, "MSFC", "TacticalBelt", 500, 300);

                    }
                }
            }
            traderlist.SaveConfigFile();

        }

        static void CheckItem(int tradID, int cateID, int itemID, string strMod, string itemName, int buyValue, int sellValue)
        {
            if (traderlist.Traders[tradID].Categories[cateID].items[itemID].Name.Contains(strMod) && traderlist.Traders[tradID].Categories[cateID].items[itemID].Name.Contains(itemName))
            {
                traderlist.Traders[tradID].Categories[cateID].items[itemID].BuyValue = buyValue;
                traderlist.Traders[tradID].Categories[cateID].items[itemID].SellValue = sellValue;
            }
        }

        static void BtnLoad()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (dlg.ShowDialog().Value)
            {
                traderlist = new TraderTable(dlg.FileName);
                traderlist.LoadFile();
            }
        }

    }
}

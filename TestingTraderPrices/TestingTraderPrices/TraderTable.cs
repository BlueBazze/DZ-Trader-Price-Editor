using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;

namespace TestingTraderPrices
{
    
    class TraderCollection : ObservableCollection<Trader> { }
    class TraderTable
    {
        public string ConfigFilePath { get; private set; }
        public TraderCollection Traders = new TraderCollection();


        public TraderTable(string file)
        {
            ConfigFilePath = file;
        }

        public TraderTable()
        {
            //Traders.Add(new Trader(new XElement("Name", "Joe", new XElement("Categories", ""))));
        }

        public void LoadFile()
        {
            ReadConfigFile(ConfigFilePath);
        }

        public void SaveConfigFile()
        {
            FileInfo file = new FileInfo(ConfigFilePath);


            if (file.Exists)
            {
                FileInfo backup = new FileInfo(file.FullName + ".original.txt");

                if (!backup.Exists)
                {
                    file.CopyTo(backup.FullName);
                }
            }

            

            List<string> lineToWrite = new List<string>();

            for (int trad = 0; trad < Traders.Count; trad++)
            {
                if (Traders[trad].Name == null)
                    continue;
                lineToWrite.Add("<Trader> " + Traders[trad].Name);

                for (int cate = 0; cate < Traders[trad].Categories.Count; cate++)
                {
                    if (Traders[trad].Categories[cate].Name == null)
                        continue;
                    lineToWrite.Add("\t<Category> " + Traders[trad].Categories[cate].Name);

                    for (int item = 0; item < Traders[trad].Categories[cate].items.Count; item++)
                    {
                        if (Traders[trad].Categories[cate].items[item].Name == null)
                            continue;

                        if(Traders[trad].Categories[cate].items[item].Quantity.Length < 1)
                            if(!Regex.IsMatch(Traders[trad].Categories[cate].items[item].Quantity, @"[M|W|S|K|^VNK$|^\d+$|V|\*]"))
                            {
                                Console.WriteLine("\nCant read item in trader {0} -> {1} -> {2}", Traders[trad].Name, Traders[trad].Categories[cate].Name, Traders[trad].Categories[cate].items[item].Name);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Item will not show up in game");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        else
                            {
                                Console.WriteLine("\nItem Quantity is null {0} -> {1} -> {2}", Traders[trad].Name, Traders[trad].Categories[cate].Name, Traders[trad].Categories[cate].items[item].Name);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Item will not show up ingame");
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                        lineToWrite.Add("\t\t" +
                            (Traders[trad].Categories[cate].items[item].Enabled ? "//" : "") + 
                            (Traders[trad].Categories[cate].items[item].Name + ",").PadRight(40) +
                            (Traders[trad].Categories[cate].items[item].Quantity.ToString() + ",").PadRight(10) +
                            (Traders[trad].Categories[cate].items[item].BuyValue + ",").PadRight(10) +
                            Traders[trad].Categories[cate].items[item].SellValue.ToString().PadRight(15) +
                            (String.IsNullOrEmpty(Traders[trad].Categories[cate].items[item].Comment) ? "" : "//" + Traders[trad].Categories[cate].items[item].Comment));
                    }
                    lineToWrite.Add("");
                }
                lineToWrite.Add("");
            }

            TextWriter tw = new StreamWriter(ConfigFilePath);

            tw.Write("<CurrencyName> #tm_ruble \n" +
                "\t<Currency> MoneyRuble1, 1 \n" +
                "\t<Currency> MoneyRuble5, 5 \n" +
                "\t<Currency> MoneyRuble10, 10 \n" +
                "\t<Currency> MoneyRuble25, 25 \n" +
                "\t<Currency> MoneyRuble50, 50 \n" +
                "\t<Currency> MoneyRuble100, 100 \n\n\n");

            foreach (string line in lineToWrite)
                tw.WriteLine(line);

            tw.WriteLine("<FileEnd>");

            tw.Flush();
            tw.Close();


        }
        
        public Trader FindByName(string name)
        {
            foreach (Trader trad in Traders)
                if (trad.Name == name)
                    return trad;
            return null;
        }

        public void ReadConfigFile(string filePath)
        {
            Traders = new TraderCollection();
            //MessageBox.Show("Reading file");
            //string[] configFile = File.ReadAllLines(filePath);
            string TraderName = "";
            string Category = "";

            int tradIndex = -1;
            int cateIndex = -1;

            List<string> errors = new List<string>();

            foreach(string line in File.ReadLines(filePath))
            {
                if (line.Contains("<Trader>"))
                {
                    TraderName = line.Remove(0, 9);
                    Traders.Add(new Trader(line.Remove(0, 9)));
                    tradIndex++;
                    cateIndex = -1;

                }
                if (line.Contains("<Category>"))
                {
                    Category = line.Remove(0, 11);
                    foreach (Trader trad in Traders)
                    {
                        if (trad.Name == TraderName)
                        {
                            trad.newCategory(line.Remove(0, 11).Replace(" ", string.Empty).Replace("\t", string.Empty));
                            cateIndex++;
                        }
                    }
                }

                if (TraderName == "" || Category == "" || line.Contains("<Currency>") || line.Contains("<CurrencyName>"))
                    continue;

                if (line.Contains(","))
                {
                    string Line = Regex.Replace(line, "\t", "");
                    Line = Line.Replace("\t", String.Empty);
                    Line = Line.Replace(" ", String.Empty);
                    string[] content = Line.Split(',');
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("\nAdded item: ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(content[0]);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" to ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Traders[tradIndex].Name);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" in ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(Traders[tradIndex].Categories[cateIndex].Name + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        /*
                        "Q: " + content[1] + " - " +
                        "B: " + Convert.ToInt32(content[2]).ToString() + " - " +
                        "S: " + (content[3].Contains("//") ? Convert.ToInt32(content[3].Split("//")[0]).ToString() : Convert.ToInt32(content[3]).ToString()) + " - " +
                        "E: " + (content[0].StartsWith("//") ? "false" : "true") + " - " +
                        "C: " + (content[3].Contains("//") ? "true" : "false") + "\n");*/

                        Traders[tradIndex].Categories[cateIndex].items.Add(new Item(
                                    content[0].StartsWith("//") ? content[0].Remove(0, 2) : content[0],
                                    content[1],
                                    Convert.ToInt32(content[2]),
                                    content[3].Contains("//") ? Convert.ToInt32(Regex.Split(content[3], "//")[0]) : Convert.ToInt32(content[3]),
                                    content[0].StartsWith("//") ? true : false,
                                    content[3].Contains("//") ? content[3].Split()[1].ToString() : ""
                                    ));
                        
                    }
                    catch (Exception e)
                    {
                        List<string> tempError = new List<string>();
                        if (Regex.Matches(line, ",").Count != 3)
                            errors.Add((Regex.Matches(line, ",").Count < 3 ? "\nError because of missing" : "\nError because of too many") + " ',' at\nline " + line + ": " + line.Replace("\t", String.Empty).Replace(" ", string.Empty));
                        else
                            errors.Add("\nSkipped item because of error at\nline " + line + ": " + line.Replace("\t", String.Empty).Replace(" ", string.Empty) + "\n(Unknown error): " + e.Message + "\n");
                    }
                }

            /*for (int line = 0; line < configFile.Length; line++)
            {
                
                if (configFile[line].Contains("<Trader>"))
                {
                    TraderName = configFile[line].Remove(0, 9);
                    Traders.Add(new Trader(configFile[line].Remove(0, 9)));
                    tradIndex++;
                    cateIndex = -1;

                }
                if (configFile[line].Contains("<Category>"))
                {
                    Category = configFile[line].Remove(0, 11);
                    foreach (Trader trad in Traders)
                    {
                        if (trad.Name == TraderName)
                        {
                            trad.newCategory(configFile[line].Remove(0, 11).Replace(" ", string.Empty).Replace("\t", string.Empty));
                            cateIndex++;
                        }
                    }
                }

                if (TraderName == "" || Category == "" || configFile[line].Contains("<Currency>") || configFile[line].Contains("<CurrencyName>"))
                    continue;

                if (configFile[line].Contains(","))
                {
                    string Line = Regex.Replace(configFile[line], "\t", "");
                    Line = Line.Replace("\t", String.Empty);
                    Line = Line.Replace(" ", String.Empty);
                    string[] content = Line.Split(',');
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("\nAdded item: ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(content[0]);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" to ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Traders[tradIndex].Name);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" in ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(Traders[tradIndex].Categories[cateIndex].Name + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        /*
                        "Q: " + content[1] + " - " +
                        "B: " + Convert.ToInt32(content[2]).ToString() + " - " +
                        "S: " + (content[3].Contains("//") ? Convert.ToInt32(content[3].Split("//")[0]).ToString() : Convert.ToInt32(content[3]).ToString()) + " - " +
                        "E: " + (content[0].StartsWith("//") ? "false" : "true") + " - " +
                        "C: " + (content[3].Contains("//") ? "true" : "false") + "\n");

                        Traders[tradIndex].Categories[cateIndex].items.Add(new Item(
                                    content[0].StartsWith("//") ? content[0].Remove(0, 2) : content[0],
                                    content[1],
                                    Convert.ToInt32(content[2]),
                                    content[3].Contains("//") ? Convert.ToInt32(content[3].Split("//")[0]) : Convert.ToInt32(content[3]),
                                    content[0].StartsWith("//") ? true : false,
                                    content[3].Contains("//") ? content[3].Split()[1].ToString() : ""
                                    ));
                    } catch (Exception e)
                    {
                        List<string> tempError = new List<string>();
                        if (Regex.Matches(configFile[line], ",").Count != 3)
                            errors.Add((Regex.Matches(configFile[line], ",").Count < 3 ? "\nError because of missing" : "\nError because of too many") + " ',' at\nline " + line + ": " + configFile[line].Replace("\t", String.Empty).Replace(" ", string.Empty));
                        else
                            errors.Add("\nSkipped item because of error at\nline " + line + ": " + configFile[line].Replace("\t", String.Empty).Replace(" ", string.Empty) + "\n(Unknown error): " + e.Message + "\n");
                    }

                    /*foreach (Category cate in FindByName(TraderName).Categories)
                    {
                        Console.WriteLine("1");
                        if (cate.Name == Category)
                        {
                            //Regex.Replace(content[0], "	", "")
                            Console.WriteLine(
                                content[0] + " - " +
                                content[1] + " - " +
                                Convert.ToInt32(content[2]).ToString() + " - " +
                                (content[3].Contains("//") ? Convert.ToInt32(content[3].Split("//")[0]).ToString() : Convert.ToInt32(content[3]).ToString()) + " - " +
                                (content[0].StartsWith("//") ? "Enabled: false" : "Enabled: true") + " - " +
                                (content[3].Contains("//") ? "Comment: true" : "Comment: false"));



                            cate.items.Add(new Item(
                                content[0],
                                content[1],
                                Convert.ToInt32(content[2]),
                                content[3].Contains("//") ? Convert.ToInt32(content[3].Split("//")[0]) : Convert.ToInt32(content[3]),
                                content[0].StartsWith("//") ? true : false,
                                content[3].Contains("//") ? content[3].Split()[1].ToString() : ""
                                ));
                            //MessageBox.Show(line);
                        }
                        Console.WriteLine(cate.Name + " - " + Category);
                    }

                }*/
                
            }
            foreach(string error in errors)
            {
                Console.WriteLine(error);
            }
            
            //MessageBox.Show("Done");
        }

    }
}

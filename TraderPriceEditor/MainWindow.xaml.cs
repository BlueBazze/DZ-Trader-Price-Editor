using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Xml.Linq;
using System.IO;
using System.Windows.Controls.Primitives;

namespace TraderPriceEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TraderTable LootTable;
        public delegate Point GetDragDropPosition(IInputElement inputElement);

        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            try
            {
                Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
                Point theMousePos = pos((IInputElement)theTarget);
                return posBounds.Contains(theMousePos);
            } catch { }
            return false;
        }

        private DataGridRow GetDataGridRowItem(int index, DataGrid dg)
        {
            if (dg.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return dg.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos, DataGrid dg)
        {
            int index = -1;
            for(int i = 0; i < dg.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i, dg);
                if(IsTheMouseOnTargetRow(itm, pos))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        int prevRowIndex = -1;

        void dgOrganiser_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prevRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition, sender as DataGrid);

            if (prevRowIndex < 0)
                return;
            (sender as DataGrid).SelectedIndex = prevRowIndex;


            //Trader drag & drop
            if ((sender as DataGrid).Name == datagridTraders.Name)
            {
                Trader selectedIndex = (sender as DataGrid).Items[prevRowIndex] as Trader;
                if (selectedIndex == null)
                    return;
                DragDropEffects dragDropEffects = DragDropEffects.Move;
                if (DragDrop.DoDragDrop(datagridTraders, selectedIndex, dragDropEffects) != DragDropEffects.None)
                    datagridTraders.SelectedItem = selectedIndex;
            }

            //Category drag & drop
            if ((sender as DataGrid).Name == datagridCategories.Name)
            {
                Category selectedIndex = (sender as DataGrid).Items[prevRowIndex] as Category;
                if (selectedIndex == null)
                    return;
                DragDropEffects dragDropEffects = DragDropEffects.Move;
                if (DragDrop.DoDragDrop(datagridCategories, selectedIndex, dragDropEffects) != DragDropEffects.None)
                    datagridCategories.SelectedItem = selectedIndex;
            }

            //Item drag & drop
            if ((sender as DataGrid).Name == datagridItems.Name)
            {
                Item selectedTrader = (sender as DataGrid).Items[prevRowIndex] as Item;
                if (selectedTrader == null)
                    return;
                DragDropEffects dragDropEffects = DragDropEffects.Move;
                if (DragDrop.DoDragDrop(datagridItems, selectedTrader, dragDropEffects) != DragDropEffects.None)
                    datagridItems.SelectedItem = selectedTrader;
            }

        }



        void dg_drop(object sender, DragEventArgs e)
        {
            //Console.WriteLine((sender as DataGrid).Name.ToString());
            if (prevRowIndex < 0)
                return;

            int index = this.GetDataGridItemCurrentRowIndex(e.GetPosition, (sender as DataGrid));

            if (index < 0)
                return;

            if (index == prevRowIndex)
                return;

            if(index == (sender as DataGrid).Items.Count - 1)
            {
                MessageBox.Show("This row cannot be used for Drop Operations");
                return;
            }



            //Trader drag & drop
            if ((sender as DataGrid).Name == datagridTraders.Name)
            {
                LootTable.Traders.Move(prevRowIndex, index);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\nPlacing Trader ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(LootTable.Traders[datagridTraders.SelectedIndex].Name);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" as # ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write((index + 1) + "\n");
            }

            //Category drag & drop
            if ((sender as DataGrid).Name == datagridCategories.Name)
            {
                LootTable.Traders[datagridTraders.SelectedIndex].Categories.Move(prevRowIndex, index);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\nPlacing Category ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].Name);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" as # ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write((index + 1));
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" in ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(LootTable.Traders[datagridTraders.SelectedIndex].Name + "\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            //Item drag & drop
            if ((sender as DataGrid).Name == datagridItems.Name)
            {
                LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].items.Move(prevRowIndex, index);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\nPlacing item ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].items[prevRowIndex].Name);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" as # ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write((index + 1));
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" in ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].Name + "\n");
                Console.ForegroundColor = ConsoleColor.White;

            }


        }

        
        public MainWindow()
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            InitializeComponent();
            
        }

        private void BtnLoad(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (dlg.ShowDialog().Value)
            {
                LootTable = new TraderTable(dlg.FileName);
                LootTable.LoadFile();

                datagridTraders.ItemsSource = LootTable.Traders;

                datagridTraders.SelectedIndex = 0;
            }
        }

        private void datagridTraders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkForEmptyLastRow();
            if ((sender as DataGrid).SelectedIndex == -1)
                (sender as DataGrid).SelectedIndex = 0;
            if (datagridTraders.SelectedCells.Count == 1 && datagridTraders.SelectedIndex != LootTable.Traders.Count)
            {
                datagridCategories.ItemsSource = LootTable.Traders[datagridTraders.SelectedIndex].Categories;
                Console.WriteLine("Selected Trader: " + LootTable.Traders[datagridTraders.SelectedIndex].Name);
            }
        }

        private void datagridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkForEmptyLastRow();
            if (datagridCategories.SelectedIndex > LootTable.Traders[datagridTraders.SelectedIndex].Categories.Count - 1 || datagridCategories.SelectedIndex < 0)
                return;

            //Console.WriteLine(datagridCategories.SelectedIndex);
            if (datagridCategories.SelectedIndex != -1)
                datagridItems.ItemsSource = LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].items;
            else
                datagridCategories.SelectedIndex = 0;
            
            if(LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].Name != string.Empty)
                Console.WriteLine("Selected Category: " + LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].Name);

            if (LootTable.Traders[datagridTraders.SelectedIndex].Categories[(sender as DataGrid).SelectedIndex].items.Count == 0)
                checkForEmptyLastRow();
            
        }

        private void BtnSave(object sender, RoutedEventArgs e)
        {
            LootTable.SaveConfigFile();
        }

        /*private void datagridCategories_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Console.WriteLine(datagridCategories.SelectedIndex);

            //datagridItems.ItemsSource = LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].items;
        }*/

        private void checkForEmptyLastRow()
        {
            if (datagridTraders.SelectedIndex == -1)
                datagridTraders.SelectedIndex = 0;
            if (datagridCategories.SelectedIndex == -1)
                datagridCategories.SelectedIndex = 0;
            if (datagridItems.SelectedIndex == -1)
                datagridItems.SelectedIndex = 0;

            Console.WriteLine(LootTable.Traders.Last().Name);
            if (LootTable.Traders[LootTable.Traders.Count - 1].Name != null)
            {
                LootTable.Traders.Add(new Trader());
                
            }

            //Console.WriteLine(datagridTraders.SelectedIndex + " | " + LootTable.Traders.Count);
            //Console.WriteLine(datagridCategories.SelectedIndex + " | " + LootTable.Traders[datagridTraders.SelectedIndex].Categories.Count);

            if (LootTable.Traders[datagridTraders.SelectedIndex].Categories.Last().Name != null)
                LootTable.Traders[datagridTraders.SelectedIndex].Categories.Add(new Category());

            try
            {
                if (LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].items.Last().Name != null)
                    LootTable.Traders[datagridTraders.SelectedIndex].Categories[datagridCategories.SelectedIndex].items.Add(new Item());
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            /*for (int trad = 0; trad < LootTable.Traders.Count; trad++)
            {
                if (LootTable.Traders[trad].Categories.Last().Name != "")
                    LootTable.Traders[trad].Categories.Add(new Category(""));

                for(int cate = 0; cate < LootTable.Traders[trad].Categories.Count; cate++)
                {
                    
                    if (LootTable.Traders[trad].Categories[cate].items[LootTable.Traders.Name != "")
                        LootTable.Traders[trad].Categories[cate].items.Add(new Item());
                }
            }*/

        }

        private void datagridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkForEmptyLastRow();
        }
        private void datagridTraders_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            checkForEmptyLastRow();
        }

        private void datagrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                (sender as DataGrid).RowEditEnding -= datagrid_RowEditEnding;
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).Items.Refresh();
                (sender as DataGrid).RowEditEnding += datagrid_RowEditEnding;
            }
            else return;

            //(sender as DataGrid).SelectedIndex = (sender as DataGrid).SelectedIndex - 1;
            
            checkForEmptyLastRow();
            //datagridTraders.SelectedItem -= 1;
        }

        private void datagridItems_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            checkForEmptyLastRow();
        }
    }
}

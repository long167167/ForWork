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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace CorporateActionCalendarStory
{
    /// <summary>
    /// Interaction logic for ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window
    {
        public ImportDialog()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            //write code to import
            ConstituentImport(txtParentTicker.Text, txtChildTicker.Text);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Close the popup window
            this.Close();
            
        }
        public void ConstituentImport(string parentTicker, string childTicker)
        {
            //Open File
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".txt", Filter = "TXT Files (*.txt)|*.txt|PRN Files (*.prn)|*.prn|CSV Files (*.csv)|*.csv", Title = "Choose a CA Notebook File."
            };
            Nullable<bool> result = dlg.ShowDialog();
            //Search for line
            using (StreamReader sr = File.OpenText(dlg.FileName))
            {
                string s1 = SearchFile(parentTicker, sr);
                string s2 = String.Empty;
                if (s1 == String.Empty)
                {
                    MessageBox.Show("The provided parent ticker was not found in the file.", "Error: Ticker not found");
                }
                if (childTicker != String.Empty)
                {
                    s2 = SearchFile(childTicker, sr);
                    if (s1 == String.Empty)
                    {
                        MessageBox.Show("The provided child ticker was not found in the file.", "Error: Ticker not found");
                    }
                    PopulateTable(s1, s2);
                }
                else
                {
                    PopulateTable(s1);
                }
            }
        }
        public string SearchFile(string ticker, StreamReader sr)
        {
            string s = String.Empty;
            while ((s=sr.ReadLine()) != null)
            {
                if (ticker == s.Split('|')[2])
                {
                    return s;
                }
            }
            return String.Empty;

        }
        public static void PopulateTable(string fileLine)
        {
            string[] data = fileLine.Split('|');
            ((MainWindow)Application.Current.MainWindow).parentCompanyName.Text = data[4];
            ((MainWindow)Application.Current.MainWindow).parentTicker.Text = data[2];
            ((MainWindow)Application.Current.MainWindow).parentTSO.Text = data[9];
            ((MainWindow)Application.Current.MainWindow).parentFloat.Text = $"{(1 - MainWindow.ConvertToDouble(data[11]))}";
            ((MainWindow)Application.Current.MainWindow).parentSize.Text = data[4];
            ((MainWindow)Application.Current.MainWindow).parentGrowth.Text = data[15];
            ((MainWindow)Application.Current.MainWindow).parentDynamic.Text = "";
            if (data[22] == "Y")
            {
                ((MainWindow)Application.Current.MainWindow).parentSP5.IsChecked = true;
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).parentSP5.IsChecked = false;
            }
        }
        public static void PopulateTable(string parentFileLine, string childFileLine)
        {
            string[] data = childFileLine.Split('|');
            var table = new TablePopulation();
            PopulateTable(parentFileLine);
            ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = data[4];
            ((MainWindow)Application.Current.MainWindow).childTicker.Text = data[2];
            ((MainWindow)Application.Current.MainWindow).childTSO.Text = data[9];
            ((MainWindow)Application.Current.MainWindow).childFloat.Text = data[12];
            ((MainWindow)Application.Current.MainWindow).childSize.Text = data[4];
            ((MainWindow)Application.Current.MainWindow).childGrowth.Text = data[15];
            ((MainWindow)Application.Current.MainWindow).childDynamic.Text = data[4];
            if (data[22] == "Y")
            {
                ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = true;
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = false;
            }
        }
    }
}

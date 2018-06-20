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
            //Call the Constituent Import method using the user input text fields
            bool Success = false;
            Success = ConstituentImport(txtParentTicker.Text, txtChildTicker.Text);
            if (Success)
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Close the popup window
            this.Close();
            
        }
        public bool ConstituentImport(string parentTicker, string childTicker)
        {
            //Display dialog for the user to choose a CA notebook file
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".txt", Filter = "TXT Files (*.txt)|*.txt|PRN Files (*.prn)|*.prn|CSV Files (*.csv)|*.csv", Title = "Choose a CA Notebook File."
            };
            Nullable<bool> result = dlg.ShowDialog();
            //Search the selected CA notebook for the two input tickers
            string s1 = String.Empty;
            string s2 = String.Empty;
            using (StreamReader sr = File.OpenText(dlg.FileName))
            {
                if (parentTicker != String.Empty)
                {
                    s1 = SearchFile(parentTicker, sr); //search for the parent ticker
                    if (s1 == String.Empty) //If the ticker is not found
                    {
                        MessageBox.Show("The provided parent ticker was not found in the file.", "Error: Ticker not found");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("You must import a parent ticker.", "Parent Needed");
                    return false;
                }
            }
            using (StreamReader sr = File.OpenText(dlg.FileName))
            {
                if (childTicker != String.Empty)//logic for importing child
                {
                    s2 = SearchFile(childTicker, sr);//search for child ticker
                    if (s2 == String.Empty)
                    {
                        MessageBox.Show("The provided child ticker was not found in the file.", "Error: Ticker not found");
                        return false;
                    }
                }
            }
            if (s1!= string.Empty && s2!=string.Empty)
            {
                PopulateTable(s1, s2);
                return true;
            }
            else if (s1 != string.Empty)
            {
                PopulateTable(s1);
                return true;
            }
            else
            {
                MessageBox.Show("Please enter a valid ticker", "Invalid Tickers Entered.");
                return false;
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
            ((MainWindow)Application.Current.MainWindow).parentSize.Text = DetermineSize(data);
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
            ((MainWindow)Application.Current.MainWindow).childFloat.Text = $"{(1 - MainWindow.ConvertToDouble(data[11]))}";
            ((MainWindow)Application.Current.MainWindow).childSize.Text = DetermineSize(data);
            ((MainWindow)Application.Current.MainWindow).childGrowth.Text = data[15];
            ((MainWindow)Application.Current.MainWindow).childDynamic.Text = "";
            if (data[22] == "Y")
            {
                ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = true;
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = false;
            }
        }
        public static string DetermineSize(string[] data)
        {
            if (data[17] == "Y" && data[19] == "Y" && data[26] == "Y")
            {
                return "L1";
            }
            else if (data[17] == "Y" && data[19] == "Y" && data[26] == "N")
            {
                return "L3";
            }
            else if (data[17] == "Y" && data[18] == "Y" && data[21] == "N")
            {
                return "M1";
            }
            else if (data[17] == "Y" && data[18] == "Y" && data[18] == "Y")
            {
                return "M2";
            }
            else if (data[20] == "Y" && data[25] == "N")
            {
                return "S1";
            }
            else if (data[20] == "Y" && data[25] == "Y")
            {
                return "S3";
            }
            else if (data[24] == "N" && data[25] == "Y")
            {
                return "MIC1";
            }
            return String.Empty;
        }
    }
}

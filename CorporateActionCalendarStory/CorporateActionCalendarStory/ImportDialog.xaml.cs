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
                    MainWindow.PopulateTable(s1, s2);
                }
                else
                {
                    MainWindow.PopulateTable(s1);
                }
            }
            //populate table
        }
        /*
        public static void PopulateTable(string fileLine)
        {
            string[] data = fileLine.Split('|');
            MainWindow mainWindow = Own
        }
        public static void PopulateTable(string fileLineParent, string fileLineChild)
        {
            PopulateTable(fileLineParent);
            //populate the table 
        }
        public void ClearChild()
        {
            //Clear the text boxes for the child
        }
        */
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
        
    }
}

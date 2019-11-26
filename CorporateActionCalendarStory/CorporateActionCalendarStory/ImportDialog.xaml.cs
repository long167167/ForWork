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


            if (((MainWindow)Application.Current.MainWindow).delistingButton.IsChecked == true) //delist story initial 
            {
                ImportTitle.Text = "Would you like to import the delisted constituent";
                parentLabel.Text = "Delisted Ticker";
                ChildTickerLabel.Text = "";
                txtChildTicker.IsEnabled = false;

            } //end of check/switch the import form to delist case

            if (((MainWindow)Application.Current.MainWindow).spinOffButton.IsChecked == true)//spin off story initial
            {
                ImportTitle.Text = "Import the constituent of Spin Off";
                parentLabel.Text = "Parent Ticker of Spin Off";
                ChildTickerLabel.Text = "";
                txtChildTicker.IsEnabled = false;

            } //end of check/switch the import form to spin off case


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

            if (((MainWindow)Application.Current.MainWindow).spinOffButton.IsChecked == true)//msgbox for noticing manually ipnut child of spin off
            {

                MessageBox.Show("Enter the Child Company Info for Spin Off");

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
                else if ((parentTicker != String.Empty)&&(((MainWindow)Application.Current.MainWindow).mergerButton.IsChecked == false))
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
            if (s1 != string.Empty && s2 != string.Empty)
            {
                PopulateTable(s1, s2);
                return true;
            }
            else if (s1 != string.Empty)
            {
                PopulateTable(s1);
                return true;
            }
            else if ((s1 == string.Empty) && (((MainWindow)Application.Current.MainWindow).mergerButton.IsChecked == true))
            {
                NonMemberTable(s2);
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
            while ((s = sr.ReadLine()) != null)
            {
                if (ticker == s.Split('|')[2])
                {
                    return s;
                }
            }
            return String.Empty;

        }

        //merger parent non member---child table 

        public static void NonMemberTable(string fileLine)
        {
            string[] data = fileLine.Split('|');

            //get company name and suffix
            string[] cName = data[4].Split(' ');
            string cSuffix = cName[cName.Length - 1];
            //check if cSuffix is been contained in the original company. if so, it will be the last item in the cName array, else the suffix dosen't exists.


            if ((cSuffix == "CORP") || (cSuffix == "INC") || (cSuffix == "CO") || (cSuffix == "LTD") || (cSuffix == "IN"))
            {
                string printName = "";
                for (int i = 0; i < (cName.Length - 1); i++)
                {
                    printName = printName + UppercaseFirst(cName[i]) + " ";
                }
                cSuffix = UppercaseFirst(cSuffix);
                ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = printName.Trim();
                ((MainWindow)Application.Current.MainWindow).childNameSuffix.Text = cSuffix;

            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = data[4].Trim();
                MessageBox.Show("Please Enter the Suffix for the parent Company");

            }


            //End of Getting company and suffix


            ((MainWindow)Application.Current.MainWindow).childTicker.Text = data[2];
            ((MainWindow)Application.Current.MainWindow).childTSO.Text = data[9];
            ((MainWindow)Application.Current.MainWindow).childFloat.Text = $"{(1 - (Convert.ToDouble(data[11])))}";
            ((MainWindow)Application.Current.MainWindow).childSize.Text = DetermineSize(data);
            ((MainWindow)Application.Current.MainWindow).childGrowth.Text = data[15];
            ((MainWindow)Application.Current.MainWindow).childDynamic.Text = "";

            //Check the exchange of the parent constituents
            if (data[5] == "V")

            { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "NASDAQ"; }

            else if (data[5] == "B")

            { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "NYSE American"; }

            else if (data[5] == "A")

            { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "NYSE"; }

            else if (data[5] == "NCBO")
            { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "CBOE"; }
            // End of getting the exchange of parent
            ////////////
            //set input stock
            //
            if (data[22] == "Y")
            {
                ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = true;
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = false;
            }

            //check parent R3
            if (data[24] == "Y")
            {
                ((MainWindow)Application.Current.MainWindow).ChildR3.IsChecked = true;
            }//end of parent R3 check


        }

        //end merger parent non member

        public static void PopulateTable(string fileLine)
        {

                string[] data = fileLine.Split('|');

                //get company name and suffix
                string[] cName = data[4].Split(' ');
                string cSuffix = cName[cName.Length - 1];
                //check if cSuffix is been contained in the original company. if so, it will be the last item in the cName array, else the suffix dosen't exists.
                
             
                if ((cSuffix == "CORP") || (cSuffix == "INC") || (cSuffix == "CO") || (cSuffix == "LTD") || (cSuffix == "IN"))
                {
                    string printName = "";
                    for (int i = 0; i < (cName.Length - 1); i++)
                    {
                        printName = printName + UppercaseFirst(cName[i]) + " ";
                    }
                    cSuffix = UppercaseFirst(cSuffix);
                    ((MainWindow)Application.Current.MainWindow).parentCompanyName.Text = printName.Trim();
                    ((MainWindow)Application.Current.MainWindow).parentNameSuffix.Text = cSuffix;

                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).parentCompanyName.Text = data[4].Trim();
                    MessageBox.Show("Please Enter the Suffix for the parent Company");

                }


            //End of Getting company and suffix


            ((MainWindow)Application.Current.MainWindow).parentTicker.Text = data[2];
                ((MainWindow)Application.Current.MainWindow).parentTSO.Text = data[9];
                ((MainWindow)Application.Current.MainWindow).parentFloat.Text = $"{(1 - (Convert.ToDouble(data[11])))}";
                ((MainWindow)Application.Current.MainWindow).parentSize.Text = DetermineSize(data);
                ((MainWindow)Application.Current.MainWindow).parentGrowth.Text = data[15];
                ((MainWindow)Application.Current.MainWindow).parentDynamic.Text = "";

                //Check the exchange of the parent constituents
                if (data[5] == "V")

                { ((MainWindow)Application.Current.MainWindow).parentExchange.Text = "NASDAQ"; }

                else if (data[5] == "B")

                { ((MainWindow)Application.Current.MainWindow).parentExchange.Text = "NYSE American"; }

                else if (data[5] == "A")

                { ((MainWindow)Application.Current.MainWindow).parentExchange.Text = "NYSE"; }

                else if (data[5] == "NCBO")
                { ((MainWindow)Application.Current.MainWindow).parentExchange.Text = "CBOE"; }
                // End of getting the exchange of parent
                ////////////
                //set input stock
                //
                if (data[22] == "Y")
                {
                    ((MainWindow)Application.Current.MainWindow).parentSP5.IsChecked = true;
                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).parentSP5.IsChecked = false;
                }

                //check parent R3
                if (data[24] == "Y")
                {
                    ((MainWindow)Application.Current.MainWindow).ParentR3.IsChecked = true;
                }//end of parent R3 check

                //insert child for Spin off
                if (((MainWindow)Application.Current.MainWindow).spinOffButton.IsChecked == true)
                {
                    if (((MainWindow)Application.Current.MainWindow).stockTermsAmount.Text == "")
                    {
                        MessageBox.Show("Warning: Please Enter the Stock Term for Spin off. And then Re-import the parent constituent.");
                        return;
                    }

                        ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = "";
                    ((MainWindow)Application.Current.MainWindow).childTicker.Text = "";

                    ((MainWindow)Application.Current.MainWindow).childTSO.Text = spinChiTSO(((MainWindow)Application.Current.MainWindow).parentTSO.Text, ((MainWindow)Application.Current.MainWindow).stockTermsAmount.Text);
                    ((MainWindow)Application.Current.MainWindow).childFloat.Text = ((MainWindow)Application.Current.MainWindow).parentFloat.Text;
                    ((MainWindow)Application.Current.MainWindow).childSize.Text = ((MainWindow)Application.Current.MainWindow).parentSize.Text;
                    ((MainWindow)Application.Current.MainWindow).childGrowth.Text = ((MainWindow)Application.Current.MainWindow).parentGrowth.Text;
                    ((MainWindow)Application.Current.MainWindow).childDynamic.Text = ((MainWindow)Application.Current.MainWindow).parentDynamic.Text;
                }//end insert Spin-Off child info 


        }

        public static string spinChiTSO(string parentTSO, string stockTerm)
        {

            double newTSO = Convert.ToDouble(parentTSO) * Convert.ToDouble(stockTerm);
            long  printTSO = Convert.ToInt64(newTSO);
            return Convert.ToString(printTSO);
        }

        public static void PopulateTable(string parentFileLine, string childFileLine)
        {
            string[] data = childFileLine.Split('|');
            var table = new TablePopulation();
            PopulateTable(parentFileLine);

               ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = data[4];
                ((MainWindow)Application.Current.MainWindow).childTicker.Text = data[2];
                ((MainWindow)Application.Current.MainWindow).childTSO.Text = data[9];
                ((MainWindow)Application.Current.MainWindow).childFloat.Text = $"{(1 - (Convert.ToDouble(data[11])))}";
                ((MainWindow)Application.Current.MainWindow).childSize.Text = DetermineSize(data);
                ((MainWindow)Application.Current.MainWindow).childGrowth.Text = data[15];
                ((MainWindow)Application.Current.MainWindow).childDynamic.Text = "";

                //get Child company name and suffix
                string[] cName = data[4].Split(' ');
                string cSuffix = cName[cName.Length - 1];
                //check if cSuffix is been contained in the original company. if so, it will be the last item in the cName array, else the suffix dosen't exists.
                if ((cSuffix == "CORP") || (cSuffix == "INC") || (cSuffix == "CO") || (cSuffix == "LTD") || (cSuffix == "IN"))
                {
                    string printName = "";
                    for (int i = 0; i < (cName.Length - 1); i++)
                    {
                        printName = printName + UppercaseFirst(cName[i]) + " ";
                    }
                    cSuffix = UppercaseFirst(cSuffix);
                    ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = printName.Trim();
                    ((MainWindow)Application.Current.MainWindow).childNameSuffix.Text = cSuffix;

                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = data[4].Trim();
                    MessageBox.Show("Please Enter the Suffix for the parent Company");

                }// End of getting child company and suffix



                //Check the exchange of the parent constituents
                if (data[5] == "V")

                { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "NASDAQ"; }

                else if (data[5] == "B")

                { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "NYSE American"; }

                else if (data[5] == "A")

                { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "NYSE"; }

                else if (data[5] == "NCBO")
                { ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "CBOE"; }
                // End of getting the exchange of parent
                ////////////





                if (data[22] == "Y")//child RSCC check
                {
                    ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = true;
                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = false;
                } //end Child RSCC check


                //check Child R3
                if (data[24] == "Y")
                {
                    ((MainWindow)Application.Current.MainWindow).ChildR3.IsChecked = true;
                }//end of Child R3 check



            

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


        //fuction for uppercase
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            string lowerpart = (s.Substring(1)).ToLower();
            return char.ToUpper(s[0]) + lowerpart;
        }
    }
}

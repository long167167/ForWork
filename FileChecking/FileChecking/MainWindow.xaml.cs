using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileChecking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch myTimer = new Stopwatch();
            myTimer.Start();
            string Date = DateInput.Text;
            string fileDirectory = filePathInput.Text;
            ExecuteCode(Date, fileDirectory);
            myTimer.Stop();
            MessageBox.Show(myTimer.Elapsed.ToString());
        }
        static void ExecuteCode(string inputDate, string fileDirectory)
        {
            //string fileDirectory = @"C:\source\datafiles\Holdings File Read\";
            //MessageBox.Show("Working..");
            string Date = inputDate;//"20180208";//Use this for testing
            List < FileArray > openFileArray = new List<FileArray>();
            List<FileArray> closeFileArray = new List<FileArray>();
            bool fileOpenError = false;
            try
            {
                openFileArray = OpenConsFile(fileDirectory + "H_OPEN_R3000E_" + Date + "_RGS.PRIME.TXT");
                try
                {
                    closeFileArray = CloseConsFile(fileDirectory + "H_" + Date + "_RGS_R3000E.PRIME.TXT");
                }
                catch (FileNotFoundException)
                {
                    fileOpenError = true;
                    MessageBox.Show("The close file does not exist");
                }
            }
            catch (FileNotFoundException)
            {
                fileOpenError = true;
                MessageBox.Show("The open file does not exist");
            }
            if (fileOpenError)
            {
                MessageBox.Show("Try checking for the file or correcting the date input.");
            }
            else
            {
                List<DiffArray> Diffs = new List<DiffArray>();
                Diffs.AddRange(CompareFiles(openFileArray, closeFileArray));
                if (Diffs.Count == 0)
                {
                    MessageBox.Show("No Differences");
                    Console.ReadLine();
                }
                else
                {
                    fileExport(Diffs, fileDirectory + "Differences_", Date);
                    MessageBox.Show("Diff file has been exported to " + fileDirectory + "Differences_" + Date + ".txt");
                }
            }
            
        }
        static List<FileArray> OpenConsFile(string filePath)
        {
            string fileType = "Open";
            //Read the file into a string array
            try
            {
                string[] myFileLines = myFileImport(filePath);
                List<FileArray> fileArray = new List<FileArray>();
                for (int i = 0; i < myFileLines.Length - 10; i++)
                {
                    fileArray.Add(DelimitFile(myFileLines[i + 2], fileType));
                }
                return fileArray;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            

        }
        static List<FileArray> CloseConsFile(string filePath)
        {
            try
            {
                //Turn the close file into a list
                string fileType = "Close";
                //Read the file into a string array
                string[] myFileLines = myFileImport(filePath);
                List<FileArray> fileArray = new List<FileArray>();
                for (int i = 0; i < myFileLines.Length - 10; i++)
                {
                    fileArray.Add(DelimitFile(myFileLines[i + 2], fileType));
                }
                return fileArray;
            }
            catch (FileNotFoundException)
            {
                throw;
            }

        }
        static List<DiffArray> CompareFiles(List<FileArray> openFileArray, List<FileArray> closeFileArray)
        {
            List<DiffArray> Diffs = new List<DiffArray>();
            bool noMatch = true;
            foreach (FileArray item in openFileArray)
            {
                noMatch = true;
                foreach (FileArray item2 in closeFileArray)
                {
                    if (item.Cusip == item2.Cusip)
                    {
                        //MessageBox.Show("you found a match!"); //replace with data comparison for each line
                        Diffs.AddRange(CompareSecurity(item, item2));
                        noMatch = false;
                        break;
                    }
                }
                if (noMatch)
                {
                    Diffs.Add(new DiffArray() { CUSIP = item.Cusip, OpenValue = "Exists", CloseValue = "Does not Exist", Field = "All" });
                }
            }
            return Diffs;
        }
        static List<DiffArray> CompareSecurity(FileArray openSecurity, FileArray closeSecurity)
        {
            List<DiffArray> Diffs = new List<DiffArray>();
            if (openSecurity.fileDate == closeSecurity.fileDate)
            {
                //dates match
            }
            else
            {
                //dates don't match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "fileDate", OpenValue = openSecurity.fileDate.ToString(), CloseValue = closeSecurity.fileDate.ToString() });
            }
            if (openSecurity.ISIN == closeSecurity.ISIN)
            {
                //ISIN Matches
            }
            else
            {
                //ISIN does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "ISIN", OpenValue = openSecurity.ISIN.ToString(), CloseValue = closeSecurity.ISIN.ToString() });
            }
            if (openSecurity.Ticker == closeSecurity.Ticker)
            {
                //Ticker matches
            }
            else
            {
                //Ticker does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "Ticker", OpenValue = openSecurity.Ticker.ToString(), CloseValue = closeSecurity.Ticker.ToString() });
            }
            if (openSecurity.Exchange == closeSecurity.Exchange)
            {
                //Exchange match
            }
            else
            {
                //Exchange does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "Exchange", OpenValue = openSecurity.Exchange.ToString(), CloseValue = closeSecurity.Exchange.ToString() });
            }
            if (openSecurity.Shares == closeSecurity.Shares)
            {
                //Shares match
            }
            else
            {
                //Shares does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "Shares", OpenValue = openSecurity.Shares.ToString(), CloseValue = closeSecurity.Shares.ToString() });
            }
            if (openSecurity.GroShares == closeSecurity.GroShares)
            {
                //Growth shares match
            }
            else
            {
                //Growth shares does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "GroShares", OpenValue = openSecurity.GroShares.ToString(), CloseValue = closeSecurity.GroShares.ToString() });
            }
            if (openSecurity.ValShares == closeSecurity.ValShares)
            {
                //Value shares match
            }
            else
            {
                //Value shares does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "ValShares", OpenValue = openSecurity.ValShares.ToString(), CloseValue = closeSecurity.ValShares.ToString() });
            }
            if (openSecurity.Russell1000Flag == closeSecurity.Russell1000Flag)
            {
                //R1 flag match
            }
            else
            {
                //R1 Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "Russell1000Flag", OpenValue = openSecurity.Russell1000Flag.ToString(), CloseValue = closeSecurity.Russell1000Flag.ToString() });
            }
            if (openSecurity.Russell2000Flag == closeSecurity.Russell2000Flag)
            {
                //R2 flag match
            }
            else
            {
                //R2 Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "Russell2000Flag", OpenValue = openSecurity.Russell2000Flag.ToString(), CloseValue = closeSecurity.Russell2000Flag.ToString() });
            }
            if (openSecurity.Russell2500Flag == closeSecurity.Russell2500Flag)
            {
                //R25 flag match
            }
            else
            {
                //R25 Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "Russell2500Flag", OpenValue = openSecurity.Russell2500Flag.ToString(), CloseValue = closeSecurity.Russell2500Flag.ToString() });
            }
            if (openSecurity.RussellMidFlag == closeSecurity.RussellMidFlag)
            {
                //Rmidcap flag match
            }
            else
            {
                //Rmidcap Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "RussellMidFlag", OpenValue = openSecurity.RussellMidFlag.ToString(), CloseValue = closeSecurity.RussellMidFlag.ToString() });
            }
            if (openSecurity.RussellTop2Flag == closeSecurity.RussellTop2Flag)
            {
                //T2 flag match
            }
            else
            {
                //T2 Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "RussellTop2Flag", OpenValue = openSecurity.RussellTop2Flag.ToString(), CloseValue = closeSecurity.RussellTop2Flag.ToString() });
            }
            if (openSecurity.RSCCFlag == closeSecurity.RSCCFlag)
            {
                //RSCCFlag flag match
            }
            else
            {
                //RSCCFlag Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "RSCCFlag", OpenValue = openSecurity.RSCCFlag.ToString(), CloseValue = closeSecurity.RSCCFlag.ToString() });
            }
            if (openSecurity.Russell3000Flag == closeSecurity.Russell3000Flag)
            {
                //R3 flag match
            }
            else
            {
                //R3 Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "Russell3000Flag", OpenValue = openSecurity.Russell3000Flag.ToString(), CloseValue = closeSecurity.Russell3000Flag.ToString() });
            }
            if (openSecurity.RussellMicroFlag == closeSecurity.RussellMicroFlag)
            {
                //Micro flag match
            }
            else
            {
                //Micro Flag does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "RussellMicroFlag", OpenValue = openSecurity.RussellMicroFlag.ToString(), CloseValue = closeSecurity.RussellMicroFlag.ToString() });
            }
            if (openSecurity.ShareChg == closeSecurity.ShareChg)
            {
                //ShareChg match
            }
            else
            {
                //ShareChg does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "ShareChg", OpenValue = openSecurity.ShareChg.ToString(), CloseValue = closeSecurity.ShareChg.ToString() });
            }
            if (openSecurity.CompanyName == closeSecurity.CompanyName)
            {
                //Names match
            }
            else
            {
                //Name does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "CompanyName", OpenValue = openSecurity.CompanyName.ToString(), CloseValue = closeSecurity.CompanyName.ToString() });
            }
            if (openSecurity.IND == closeSecurity.IND)
            {
                //RGS match
            }
            else
            {
                //RGS does not match
                Diffs.Add(new DiffArray() { CUSIP = openSecurity.Cusip, Field = "IND", OpenValue = openSecurity.IND.ToString(), CloseValue = closeSecurity.IND.ToString() });
            }
            return Diffs;
        }
        private static string[] myFileImport(string filePath)
        {
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(filePath);
                return lines;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }
        static void fileExport(List<DiffArray> Diffs, string FilePath, string Date)
        {
            using (StreamWriter sw = File.CreateText(FilePath + Date + ".txt"))
            {
                sw.WriteLine("Field, CUSIP, Close Value, OpenValue");
            }
            using (StreamWriter sw = File.AppendText(FilePath + Date + ".txt"))
            {
                foreach (DiffArray item in Diffs)
                {
                    sw.WriteLine(item.Field + ", " + item.CUSIP + ", " + item.CloseValue + ", " + item.OpenValue);
                }
            }

        }
        private static FileArray DelimitFile(string line, string fileType)
        {
            FileArray fileArray = new FileArray();
            if (fileType == "Open")
            {
                fileArray.fileDate = line.Substring(0, 8);
                fileArray.Cusip = line.Substring(9, 9);
                fileArray.ISIN = line.Substring(19, 12);
                fileArray.Ticker = line.Substring(32, 7).Trim();
                fileArray.Exchange = line.Substring(41, 12).Trim();
                fileArray.Return = Double.Parse(line.Substring(54, 7));
                fileArray.MTDReturn = Double.Parse(line.Substring(62, 10));
                fileArray.MktValue = Double.Parse(line.Substring(73, 13));
                try
                {
                    fileArray.CashDividend = Double.Parse(line.Substring(87, 13));
                }
                catch (FormatException)
                {
                    fileArray.CashDividend = 0D;
                }
                fileArray.Shares = Convert.ToInt32(line.Substring(101, 9));
                fileArray.ValShares = Convert.ToInt32(line.Substring(111, 9));
                fileArray.GroShares = Convert.ToInt32(line.Substring(121, 9));
                fileArray.Russell1000Flag = Convert.ToChar(line.Substring(131, 1));
                fileArray.Russell2000Flag = Convert.ToChar(line.Substring(133, 1));
                fileArray.Russell2500Flag = Convert.ToChar(line.Substring(135, 1));
                fileArray.RussellMidFlag = Convert.ToChar(line.Substring(137, 1));
                fileArray.RussellTop2Flag = Convert.ToChar(line.Substring(139, 1));
                fileArray.RSCCFlag = Convert.ToChar(line.Substring(141, 1));
                fileArray.Russell3000Flag = Convert.ToChar(line.Substring(143, 1));
                fileArray.RussellMicroFlag = Convert.ToChar(line.Substring(145, 5).Trim());
                fileArray.ShareChg = Convert.ToInt32(line.Substring(152, 10));
                fileArray.CompanyName = line.Substring(163, 25).Trim();
                fileArray.ES = line.Substring(189, 2);
                fileArray.EconomicSector = line.Substring(192, 30).Trim();
                fileArray.SubS = line.Substring(223, 4);
                fileArray.SubSector = line.Substring(228, 48).Trim();
                fileArray.IND = line.Substring(277, 6);
                fileArray.Industry = line.Substring(284, 49).Trim();
                //eventually import weights for evaluation
                return fileArray;
            }
            else if (fileType == "Close")
            {
                fileArray.fileDate = line.Substring(0, 8);
                fileArray.Cusip = line.Substring(9, 9);
                fileArray.ISIN = line.Substring(19, 12);
                fileArray.Ticker = line.Substring(32, 7).Trim();
                fileArray.Exchange = line.Substring(41, 12).Trim();
                fileArray.Return = Double.Parse(line.Substring(54, 7));
                fileArray.MTDReturn = Double.Parse(line.Substring(62, 10));
                fileArray.MktValue = Double.Parse(line.Substring(73, 13));

                fileArray.Shares = Convert.ToInt32(line.Substring(87, 9));
                fileArray.ValShares = Convert.ToInt32(line.Substring(97, 9));
                fileArray.GroShares = Convert.ToInt32(line.Substring(107, 9));
                fileArray.Russell1000Flag = Convert.ToChar(line.Substring(117, 1));
                fileArray.Russell2000Flag = Convert.ToChar(line.Substring(119, 1));
                fileArray.Russell2500Flag = Convert.ToChar(line.Substring(121, 1));
                fileArray.RussellMidFlag = Convert.ToChar(line.Substring(123, 1));
                fileArray.RussellTop2Flag = Convert.ToChar(line.Substring(125, 1));
                fileArray.RSCCFlag = Convert.ToChar(line.Substring(127, 1));
                fileArray.Russell3000Flag = Convert.ToChar(line.Substring(129, 1));
                fileArray.RussellMicroFlag = Convert.ToChar(line.Substring(131, 5).Trim());
                fileArray.ShareChg = Convert.ToInt32(line.Substring(138, 10));
                fileArray.CompanyName = line.Substring(149, 25).Trim();
                fileArray.ES = line.Substring(175, 2);
                fileArray.EconomicSector = line.Substring(178, 30).Trim();
                fileArray.SubS = line.Substring(209, 4);
                fileArray.SubSector = line.Substring(214, 48).Trim();
                fileArray.IND = line.Substring(263, 6);
                fileArray.Industry = line.Substring(270, 49).Trim();
                //eventually import weights for evaluation
                return fileArray;
            }
            else
            {
                MessageBox.Show("Invalid FileType");
                fileArray = null;
                return fileArray;
            }

        }
    }
    class FileArray
    {
        public string fileDate { get; set; }
        public string Cusip { get; set; }
        public string ISIN { get; set; }
        public string Ticker { get; set; }
        public string Exchange { get; set; }
        public double Return { get; set; }
        public double MTDReturn { get; set; }
        public double MktValue { get; set; }
        public double CashDividend { get; set; }
        public int Shares { get; set; }
        public int ValShares { get; set; }
        public int GroShares { get; set; }
        public char Russell1000Flag { get; set; }
        public char Russell2000Flag { get; set; }
        public char Russell2500Flag { get; set; }
        public char RussellMidFlag { get; set; }
        public char RussellTop2Flag { get; set; }
        public char RSCCFlag { get; set; }
        public char Russell3000Flag { get; set; }
        public char RussellMicroFlag { get; set; }
        public int ShareChg { get; set; }
        public string CompanyName { get; set; }
        public string ES { get; set; }
        public string EconomicSector { get; set; }
        public string SubS { get; set; }
        public string SubSector { get; set; }
        public string IND { get; set; }
        public string Industry { get; set; }
        /* Eventually this will be used to import constituent weights
        public double WTR3E { get; set; }
        public double WTR3EG { get; set; }
        public double WTR3EV { get; set; }
        public double WTR1 { get; set; }
        public double WTR1G { get; set; }
        public double WTR1V { get; set; }
        public double WTR2 { get; set; }
        public double WTR2G { get; set; }
        public double WTR2V { get; set; }
        public double WTR25 { get; set; }
        public double WTR25G { get; set; }
        public double WTR25V { get; set; }
        public double WTRMID { get; set; }
        public double WTMIDG { get; set; }
        public double WTMIDV { get; set; }
        public double WTRT2 { get; set; }
        public double WTRT2G { get; set; }
        public double WTRT2V { get; set; }
        public double WTRSSC { get; set; }
        public double WTRSSCG { get; set; }
        public double WTRSSCV { get; set; }
        public double WTR3 { get; set; }
        public double WTR3G { get; set; }
        public double WTR3V { get; set; }
        public double WTMICRO { get; set; }
        public double WTMICROG { get; set; }
        public double WTMICROV { get; set; }
        */
    }
    class DiffArray
    {
        public string CUSIP { get; set; }
        public string Field { get; set; }
        public string OpenValue { get; set; }
        public string CloseValue { get; set; }
    }
}

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
using CorporateActionCalendarStory;

namespace CorporateActionCalendarStory
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

        private void writeStory_Click(object sender, RoutedEventArgs e)
        {
            var parent = new StockInputs();
            var child = new StockInputs();
            GetStockInputs(ref parent, true);
            GetStockInputs(ref child, false);
            //Get all the inputs and check them
            bool isCash = !String.IsNullOrWhiteSpace(cashTermsAmount.Text);
            bool isStock = !String.IsNullOrWhiteSpace(stockTermsAmount.Text);

            //Find Cash and/orStock
            AndOrSymbol andOr = AndOrSymbol.neither;
            if (cashAndStock.IsChecked == true)
                andOr = AndOrSymbol.and;
            else if (cashOrStock.IsChecked == true)
                andOr = AndOrSymbol.or;
            else if (cashOrStock.IsChecked == false && cashAndStock.IsChecked == false)
                andOr = AndOrSymbol.or;
            else
                MessageBox.Show("You should not see this message");


            //add switch statement for Radio Button selection
            if (mergerButton.IsChecked == true)
            {
                if (isCash && !isStock)
                {
                    if (parent.SizeDimension == sizeDimension.Other)
                    {
                        //write story for non-member
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Not in the Russell indexes) will acquire " +
                            $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {cashTermsAmount.Text} in cash. The merger is pending {childCompanyName.Text}'s " +
                            $"shareholder meeting on {meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else if (parent.SizeDimension == sizeDimension.Global)
                    {
                        //Write Global story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Non-US Russell Index Member) will acquire " +
                            $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {cashTermsAmount.Text} in cash. The merger is pending {childCompanyName.Text}'s " +
                            $"shareholder meeting on {meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else
                    {
                        //Write US story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                        $"{MembershipFinder(child)}): " +
                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Russell Index Member) will acquire " +
                        $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                        $"will be exchanged for {cashTermsAmount.Text} in cash. The merger is pending {childCompanyName.Text}'s " +
                        $"shareholder meeting on {meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                        $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;

                    }
                }
                else if (!isCash && isStock)
                {
                    MessageBox.Show("Stock Merger");
                    long parentAvailable = FloatSharesCalculator(parent.ShareNum, parent.FloatNum);
                    long childAvailable = FloatSharesCalculator(child.ShareNum, child.FloatNum);
                    long newShares = MergerShareCalculator(parentAvailable, childAvailable, parent.StockTerms, parent.StockProrationNum);
                    double newGrowth = 100 * Math.Round(ProbabilityCalculator(MergerShareCalculator(StyleShares(parentAvailable, parent.GrowthNum), childAvailable, parent.StockTerms, child.GrowthNum), newShares), 3);
                    double newValue = 100 -newGrowth;
                    double newDefensive = 100 * Math.Round(ProbabilityCalculator(MergerShareCalculator(StyleShares(parentAvailable, parent.DefensiveNum), childAvailable, parent.StockTerms, child.DefensiveNum), newShares), 3);
                    double newDynamic = 100 - newDefensive;


                    if (parent.SizeDimension == sizeDimension.Other)
                    {
                        //write story for acquired by non-member
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Not in the Russell indexes) " +
                            $"will acquire {childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {stockTermsAmount.Text} of a share of {parentCompanyName.Text}. " +
                            $"The merger is pending {childCompanyName.Text}'s shareholder meeting on {meetingDate.Text}. " +
                            $"{childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else if (parent.SizeDimension == sizeDimension.Global)
                    {
                        //Write acquired by  Global story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Non-US Russell Index Member) will acquire " +
                            $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {cashTermsAmount.Text} in cash. The merger is pending {childCompanyName.Text}'s " +
                            $"shareholder meeting on {meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else
                    {
                        //Write US-us story
                        string styleChange = "";
                        if (newValue == 100 * Math.Round(parent.ValueNum, 3))
                        {
                            styleChange = "remain constant at";
                        }
                        else
                        {
                            styleChange = "change to";
                        }
                        string stabChange = "";
                        if (newDynamic == 100 * Math.Round(parent.DynamicNum, 3))
                        {
                            stabChange = "remain constant at";
                        }
                        else
                        {
                            stabChange = "change to";
                        }
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                        $"{MembershipFinder(child)}): " +
                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; " +
                        $"{MembershipFinder(parent)}" +
                        $") will acquire {childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                        $"will be exchanged for {stockTermsAmount.Text} of a share of {parentCompanyName.Text}. The merger is pending {childCompanyName.Text}'s " +
                        $"shareholder meeting on {meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                        $"Russell Indexes upon completion of the merger. Based on Russell's current projections, the new share total for {parentCompanyName.Text}" +
                        $" will be {newShares, 0:N0} shares. The style probabilities are expected " +
                        $"to {styleChange} {newGrowth}% growth and {newValue}% value and the stability " +
                        $"probabilities are expected to {stabChange} {newDefensive}% defensive and {newDynamic}% dynamic.";
                        storyResultsBox.Text = corporateActionStory;
                    }

                }
                else if (isCash && isStock)
                {
                    switch (andOr)
                    {
                        case AndOrSymbol.and:
                            MessageBox.Show("Stock and Cash Merger");
                            break;
                        case AndOrSymbol.or:
                            MessageBox.Show("Stock or Cash Merger");
                            break;
                        case AndOrSymbol.neither:
                            MessageBox.Show("Please select whether it is stock and cash or stock or cash.");
                            break;
                        default:
                            break;
                    }

                }

            }
            else if (spinOffButton.IsChecked == true)
            {
                //Write a spin off story
            }
            else if (delistingButton.IsChecked == true)
            {
                //write a delisting story
            }
            else if (tenderOfferButton.IsChecked == true)
            {
                //write a tender offer story
                if (parent.SizeDimension == sizeDimension.Other)
                {
                    
                    //write story for acquired by non-member

                    string corporateActionStory = $"{childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                        $"{MembershipFinder(child)}): " +
                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Not in the Russell indexes) will acquire " +
                        $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                        $"will be exchanged for {cashTermsAmount.Text} in cash. The offer is set to expire on " +
                        $"{meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                        $"Russell Indexes upon completion of the merger.";
                    storyResultsBox.Text = corporateActionStory;
                }
                else if (parent.SizeDimension == sizeDimension.Global)
                {
                    //Write acquired by  Global story
                    string corporateActionStory = $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                        $"{MembershipFinder(child)}): " +
                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Non-US Russell Index Member) will acquire " +
                        $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                        $"will be exchanged for {cashTermsAmount.Text} in cash. The offer is set to expire on " +
                        $"{meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                        $"Russell Indexes upon completion of the merger.";
                    storyResultsBox.Text = corporateActionStory;
                }
                else
                {
                    //Write US-us story
                    string corporateActionStory = $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                    $"{MembershipFinder(child)}): " +
                    $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; " +
                    $"Russell Index Member) will acquire " +
                    $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                    $"will be exchanged for {cashTermsAmount.Text} in cash. The offer is set to expire on " +
                    $"{meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                    $"Russell Indexes upon completion of the merger.";
                    storyResultsBox.Text = corporateActionStory;

                }
            }

            //Check status of fields
            //Report Issues with fields
            //Confirm user wants to execute
            //string corporateActionType = ;


            //string corporateActionStory = String.Format("{0} {1}", parentCompanyName.Text, parentNameSuffix.Text);
            //corporateActionStory += String.Format(" {0}", MembershipFinder(parent.SizeDimension));
            //corporateActionStory += String.Format(": {0} {1} {2}", childCompanyName.Text, childNameSuffix.Text, MembershipFinder(child.SizeDimension));
            //corporateActionStory +=
            //    String.Format(" will acquire {0} for cash. In the transaction, each share of {0} stock will be exchanged for {1} in cash. The merger is pending {0}’s shareholder meeting on REPLACE MEETING DATE. {0} will be removed from the Russell Indexes upon completion of the merger."
            //    , childCompanyName.Text, cashTermsAmount.Text);
            //
            //storyResultsBox.Text = corporateActionStory;
            

        }
        public double ProbabilityCalculator(long probabilityShares, long newShares)
        {
            double newProb = Convert.ToDouble(probabilityShares);
            newProb = newProb / Convert.ToDouble(newShares);
            return newProb;
        }
        public long StyleShares(long shares, double probability)
        {
            long probabilityShares = Convert.ToInt64(shares*probability);
            return probabilityShares;
        }
        public long MergerShareCalculator(long parentShares, long childShares, double terms, double stockProration)
        {
            //calculate merger shares
            long newShares = Convert.ToInt64(Math.Floor(childShares * terms * stockProration));
            newShares = parentShares + newShares;
            return newShares;
        }
        public long MergerShareCalculator(long parentShares, long childShares, double probability)
        {
            //calculate merger shares
            return Convert.ToInt64(parentShares + Math.Floor(childShares * probability));
        }
        public long FloatSharesCalculator(long shares, double investibilityWeight)
        {
            //calculate float
            long floatShares = Convert.ToInt64(Math.Floor(shares * investibilityWeight));
            return floatShares;
        }
        public string MembershipFinder(sizeDimension size)
        {
            switch (size)
            {
                case sizeDimension.L1:
                    return "R1, T2";
                case sizeDimension.L2:
                    return "R1, T2";
                case sizeDimension.L3:
                    return "R1, T2";
                case sizeDimension.L4:
                    return "R1, T2";
                case sizeDimension.M1:
                    return "R1, MC";
                case sizeDimension.M2:
                    return "R1, R25, MC";
                case sizeDimension.S1:
                    return "R2, R25";
                case sizeDimension.S2:
                    return "R2, R25";
                case sizeDimension.S3:
                    return "R2, R25";
                case sizeDimension.S4:
                    return "R2, R25";
                case sizeDimension.MIC1:
                    return "Micro Only Story";
                case sizeDimension.Other:
                    return "REPLACE WITH MEMBERSHIP PHRASE";
                default:
                    return "";
            }
        }
        public string MembershipFinder(StockInputs inputs)
        {
            string returnValue = MembershipFinder(inputs.SizeDimension);
            if (inputs.RSCC)
            {
                returnValue += ", RS";
                if (inputs.IsGrowth)
                {
                    returnValue += ", G";
                    if (inputs.IsValue)
                    {
                        returnValue += ", V";
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                    else
                    {
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                }
                else
                {
                    if (inputs.IsValue)
                    {
                        returnValue += ", V";
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                    else
                    {
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                }
            }
            else
            {
                if (inputs.IsGrowth)
                {
                    returnValue += ", G";
                    if (inputs.IsValue)
                    {
                        returnValue += ", V";
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                    else
                    {
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                }
                else
                {
                    if (inputs.IsValue)
                    {
                        returnValue += ", V";
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                    else
                    {
                        if (inputs.IsDefensive)
                        {
                            returnValue += ", DF";
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                        else
                        {
                            if (inputs.IsDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                                return returnValue;
                        }
                    }
                }
            }
        }
        public enum AndOrSymbol
        {
            and,
            or,
            neither
        };
        public enum sizeDimension
        {
            L1,
            L2,
            L3,
            L4,
            M1,
            M2,
            S1,
            S2,
            S3,
            S4,
            MIC1,
            Other,
            Global,
            US
        };
        public enum ProbabilityChage
        {
            change,
            remain
        }
        public static double ConvertToDouble(string Input)
        {
            try
            {
                if (Convert.ToDouble(Input)<= 1)
                    return Convert.ToDouble(Input);
                else
                {
                    MessageBox.Show("Style/Stabilty Ratios, float and proration must be between 0 and 1");
                    return 0.000;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Style/Stabilty Ratios, float and proration must be between 0 and 1");
                return 0.000;
            }
        }
        public long ConvertToLong(string Input)
        {
            try
            {
                return Convert.ToInt64(Input);
            }
            catch (Exception)
            {
                MessageBox.Show("Check the Share Inputs.");
                return 0;
            }
        }
        public bool CheckSP(bool? Input)
        {
            return Input ?? false;
        }
        public bool IsOne(double Input)
        {
            if (Input > 0)
                return true;
            else
                return false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new ImportDialog();
            newWindow.ShowDialog();
        }

        public static StockInputs GetStockInputs(ref StockInputs input, bool Parent)
        {
            if (Parent)
            {
                input.GrowthNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentGrowth.Text);
                input.ValueNum = 1 - input.GrowthNum;
                input.DynamicNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentDynamic.Text);
                input.DefensiveNum = 1 - input.DynamicNum;
                input.FloatNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentFloat.Text);
                input.IsGrowth = !input.GrowthNum.Equals(0.00);
                input.IsValue = !input.ValueNum.Equals(0.00);
                input.IsDefensive = !input.DefensiveNum.Equals(0.00);
                input.IsDynamic = !input.DynamicNum.Equals(0.00);
                try
                {
                    input.StockTerms = Convert.ToDouble(((MainWindow)Application.Current.MainWindow).stockTermsAmount.Text);
                }
                catch (Exception)
                {
                    input.StockTerms = 0.000;
                }
                input.StockProrationNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).stockProration.Text);
                input.CashProrationNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).cashProration.Text);
                try
                {
                    input.SizeDimension = (sizeDimension)Enum.Parse(typeof(sizeDimension), ((MainWindow)Application.Current.MainWindow).parentSize.Text);
                }
                catch (Exception)
                {
                    input.SizeDimension = sizeDimension.Other;
                }
            }
            else
            {
                input.GrowthNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).childGrowth.Text);
                input.ValueNum = 1 - input.GrowthNum;
                input.DynamicNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).childDynamic.Text);
                input.DefensiveNum = 1 - input.DynamicNum;
                input.FloatNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).childFloat.Text);
                input.IsGrowth = !input.GrowthNum.Equals(0.00);
                input.IsValue = !input.ValueNum.Equals(0.00);
                input.IsDefensive = !input.DefensiveNum.Equals(0.00);
                input.IsDynamic = !input.DynamicNum.Equals(0.00);
                try
                {
                    input.SizeDimension = (sizeDimension)Enum.Parse(typeof(sizeDimension), ((MainWindow)Application.Current.MainWindow).childSize.Text);
                }
                catch (Exception)
                {
                    input.SizeDimension = sizeDimension.Other;
                }
            }
            
            return input;
        }
    }

}

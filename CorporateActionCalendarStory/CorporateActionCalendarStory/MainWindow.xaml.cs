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
            //Get all the inputs and check them
            bool isCash = !String.IsNullOrWhiteSpace(cashTermsAmount.Text);
            bool isStock = !String.IsNullOrWhiteSpace(stockTermsAmount.Text);
            //take in probabilities and float
            double parentGrowthNum = ConvertToDouble(parentGrowth.Text);
            double parentValueNum = 1-ConvertToDouble(parentGrowth.Text);
            double parentDynamicNum = ConvertToDouble(parentDynamic.Text);
            double parentDefensiveNum = 1- ConvertToDouble(parentDynamic.Text);
            double parentFloatNum = ConvertToDouble(parentFloat.Text);
            double childGrowthNum = ConvertToDouble(childGrowth.Text);
            double childValueNum = 1-ConvertToDouble(childGrowth.Text);
            double childDynamicNum = ConvertToDouble(childDynamic.Text);
            double childDefensiveNum = 1- ConvertToDouble(childDynamic.Text);
            double childFloatNum = ConvertToDouble(childFloat.Text);
            //Take in S&P flags
            bool parentSP = CheckSP(parentSP5.IsChecked);
            bool childSP = CheckSP(childSP5.IsChecked);
            //Check TSO
            long parentShareNum = ConvertToLong(parentTSO.Text);
            long childShareNum = ConvertToLong(childTSO.Text);
            //Take stock terms
            double stockTerms = 0.00;
            try
            {
                stockTerms = Convert.ToDouble(stockTermsAmount.Text);
            }
            catch (Exception)
            {
                stockTerms = 0.000;
            }
            //Get proration
            double cashProrationNum = ConvertToDouble(cashProration.Text);
            double stockProrationNum = ConvertToDouble(stockProration.Text);
            //Determin Style/Stab
            bool parentisGrowth = IsOne(parentGrowthNum);
            bool parentisValue = IsOne(parentValueNum);
            bool parentisDefensive = IsOne(parentDefensiveNum);
            bool parentisDynamic = IsOne(parentDynamicNum);
            bool childisGrowth = IsOne(childGrowthNum);
            bool childisValue = IsOne(childValueNum);
            bool childisDefensive = IsOne(childDefensiveNum);
            bool childisDynamic = IsOne(childDynamicNum);

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
            sizeDimension parentSizeDimension = sizeDimension.Other;
            sizeDimension childSizeDimension = sizeDimension.Other;
            try
            {
                parentSizeDimension = (sizeDimension)Enum.Parse(typeof(sizeDimension), parentSize.Text);
            }
            catch (Exception)
            {
                parentSizeDimension = sizeDimension.Other;
            }
            try
            {
                childSizeDimension = (sizeDimension)Enum.Parse(typeof(sizeDimension), childSize.Text);
            }
            catch (Exception)
            {
                childSizeDimension = sizeDimension.Other;
            }




            //add switch statement for Radio Button selection
            if (mergerButton.IsChecked == true)
            {
                if (isCash && !isStock)
                {
                    if (parentSizeDimension == sizeDimension.Other)
                    {
                        //write story for non-member
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"; {MembershipFinder(childSizeDimension)}" +
                            $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Not in the Russell indexes) will acquire " +
                            $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {cashTermsAmount.Text} in cash. The merger is pending {childCompanyName.Text}'s " +
                            $"shareholder meeting on {meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else if (parentSizeDimension == sizeDimension.Global)
                    {
                        //Write Global story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"; {MembershipFinder(childSizeDimension)}" +
                            $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
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
                        $"; {MembershipFinder(childSizeDimension)}" +
                        $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
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
                    long parentAvailable = FloatSharesCalculator(parentShareNum, parentFloatNum);
                    long childAvailable = FloatSharesCalculator(childShareNum, childFloatNum);
                    long newShares = MergerShareCalculator(parentAvailable, childAvailable, stockTerms, stockProrationNum);
                    double newGrowth = 100 * Math.Round(ProbabilityCalculator(StyleShares(parentShareNum, parentGrowthNum), newShares), 3);
                    double newValue = 100 * Math.Round(ProbabilityCalculator(StyleShares(parentShareNum, parentValueNum), newShares), 3);
                    double newDefensive = 100 * Math.Round(ProbabilityCalculator(StyleShares(parentShareNum, parentDefensiveNum), newShares), 3);
                    double newDynamic = 100 * Math.Round(ProbabilityCalculator(StyleShares(parentShareNum, parentDynamicNum), newShares), 3);


                    if (parentSizeDimension == sizeDimension.Other)
                    {
                        //write story for acquired by non-member
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"; {MembershipFinder(childSizeDimension)}" +
                            $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Not in the Russell indexes) " +
                            $"will acquire {childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {stockTermsAmount.Text} of a share of {parentCompanyName.Text}. " +
                            $"The merger is pending {childCompanyName.Text}'s shareholder meeting on {meetingDate.Text}. " +
                            $"{childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else if (parentSizeDimension == sizeDimension.Global)
                    {
                        //Write acquired by  Global story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $"; {MembershipFinder(childSizeDimension)}" +
                            $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
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
                        string change = "";
                        if (StyleShares(parentShareNum, parentValueNum)== 100 * Math.Round(newValue, 3))
                        {
                            change = "remain constant at";
                        }
                        else
                        {
                            change = "change to";
                        }
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                        $"; {MembershipFinder(childSizeDimension)}" +
                        $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; " +
                        $"{MembershipFinder(parentSizeDimension)}{MembershipFinder(parentSP, parentisGrowth, parentisValue, parentisDefensive, parentisDynamic)}" +
                        $") will acquire {childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                        $"will be exchanged for {stockTermsAmount} of a share of {parentCompanyName.Text}. The merger is pending {childCompanyName.Text}'s " +
                        $"shareholder meeting on {meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                        $"Russell Indexes upon completion of the merger. Based on Russell's current projections, the new share total for {parentCompanyName.Text}" +
                        $" will be {newShares} shares. The style probabilities are expected " +
                        $"to {change} to {newGrowth}% growth and {newValue}% value and the stability " +
                        $"probabilities are expected to {change} to {newDefensive}% defensive and {newDynamic}% dynamic.";
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
                if (parentSizeDimension == sizeDimension.Other)
                {
                    
                    //write story for acquired by non-member

                    string corporateActionStory = $"{childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                        $"; {MembershipFinder(childSizeDimension)}" +
                        $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; Not in the Russell indexes) will acquire " +
                        $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                        $"will be exchanged for {cashTermsAmount.Text} in cash. The offer is set to expire on " +
                        $"{meetingDate.Text}. {childCompanyName.Text} will be removed from the " +
                        $"Russell Indexes upon completion of the merger.";
                    storyResultsBox.Text = corporateActionStory;
                }
                else if (parentSizeDimension == sizeDimension.Global)
                {
                    //Write acquired by  Global story
                    string corporateActionStory = $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                        $"; {MembershipFinder(childSizeDimension)}" +
                        $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
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
                    $"; {MembershipFinder(childSizeDimension)}" +
                    $"{MembershipFinder(childSP, childisGrowth, childisValue, childisDefensive, childisDynamic)}): " +
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
            //corporateActionStory += String.Format(" {0}", MembershipFinder(parentSizeDimension));
            //corporateActionStory += String.Format(": {0} {1} {2}", childCompanyName.Text, childNameSuffix.Text, MembershipFinder(childSizeDimension));
            //corporateActionStory +=
            //    String.Format(" will acquire {0} for cash. In the transaction, each share of {0} stock will be exchanged for {1} in cash. The merger is pending {0}’s shareholder meeting on REPLACE MEETING DATE. {0} will be removed from the Russell Indexes upon completion of the merger."
            //    , childCompanyName.Text, cashTermsAmount.Text);
            //
            //storyResultsBox.Text = corporateActionStory;
            

        }
        public double ProbabilityCalculator(long probabilityShares, long newShares)
        {
            double newProb = Convert.ToDouble(probabilityShares)/Convert.ToDouble(newShares);
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
            return Convert.ToInt64(parentShares + Math.Floor(childShares*terms*stockProration));
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
        public string MembershipFinder(bool SP, bool isGrowth, bool isValue, bool isDefensive, bool isDynamic)
        {
            string returnValue = "";
            if (SP)
            {
                returnValue += ", RS";
                if (isGrowth)
                {
                    returnValue += ", G";
                    if (isValue)
                    {
                        returnValue += ", V";
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                    }
                    else
                    {
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                    }
                }
                else
                {
                    if (isValue)
                    {
                        returnValue += ", V";
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                    }
                    else
                    {
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                    }
                }
            }
            else
            {
                if (isGrowth)
                {
                    returnValue += ", G";
                    if (isValue)
                    {
                        returnValue += ", V";
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                    }
                    else
                    {
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                    }
                }
                else
                {
                    if (isValue)
                    {
                        returnValue += ", V";
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                    }
                    else
                    {
                        if (isDefensive)
                        {
                            returnValue += ", DF";
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
                        }
                        else
                        {
                            if (isDynamic)
                            {
                                returnValue += ", DY";
                                return returnValue;
                            }
                            else
                            {
                                return returnValue;
                            }
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
        public double ConvertToDouble(string Input)
        {
            try
            {
                if (Convert.ToDouble(Input)<= 1)
                {
                    return Convert.ToDouble(Input);
                }
                else
                {
                    MessageBox.Show("Style/Stabilty Ratios, float and proration must be between 0 and 1");
                    return 0.000;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("You messed up parent growth");
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
        //public bool CheckFlag()
    }
}

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
    /// 
  
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

            string pDate ="";
            string rDate = "";

            if(tenderOfferButton.IsChecked != true)
            {//get dates for all non tender offer 
                if ((closeDate.Text == "") || (meetingDate.Text == "")) //check date is not null
                {
                    MessageBox.Show("Please Enter Action Dates");
                    return;
                }
                else
                {   // format close date  and meeting date
                    DateTime dt = DateTime.Parse(meetingDate.Text);
                    DateTime cDate = Convert.ToDateTime(meetingDate.Text);
                    pDate = cDate.ToLongDateString();

                    if ((delistingButton.IsChecked == true) || (spinOffButton.IsChecked == true))
                    {
                        dt = DateTime.Parse(closeDate.Text);
                        cDate = Convert.ToDateTime(closeDate.Text);
                        rDate = cDate.ToLongDateString();
                    }

                }//end of get date for non tender offer
   

            } // end of checking null date
            else

            { //get expiration for tender offer
                if (closeDate.Text == "") //check date is not null
                {
                    MessageBox.Show("Please Enter Expiration Dates");
                    return;
                }
                else
                {   // format close date  and meeting date
                    DateTime dt = DateTime.Parse(closeDate.Text);
                    DateTime cDate = Convert.ToDateTime(closeDate.Text);
                    pDate = cDate.ToLongDateString();

                }//end of get date for non tender offer

            }// end of checking null date for tender offer




            // check if delist button is chosen. 
            if (delistingButton.IsChecked == true)   //Delisting Story
            {  //generate the CA store for delisting case

                string otc = $"As of {pDate}, {parentCompanyName.Text} is trading Over-The-Counter(OTC) under ticker {parentTicker.Text}. ";

                if (OTCCheck.IsChecked == false)
                {
                    string corporateActionStory = $"{parentCompanyName.Text} {parentNameSuffix.Text}({parentTicker.Text}; " +
                                                $"{MembershipFinder(parent)}): The {parentExchange.Text}" +
                                                $" has moved to officially delist {parentCompanyName.Text} common stock from its exchange. " +
                                                otc +
                                                $"Russell will remove {parentCompanyName.Text} from the Russell Microcap Indexes effctive " +
                                                $"{rDate} (after the close).";
                    //insert CA story result
                    storyResultsBox.Text = corporateActionStory;
                }
                else
                {
                    string corporateActionStory = $"{parentCompanyName.Text} {parentNameSuffix.Text}({parentTicker.Text}; " +
                                                    $"{MembershipFinder(parent)}): The {parentExchange.Text}" +
                                                    $" has moved to officially delist {parentCompanyName.Text} common stock from its exchange. " +        
                                                    $"Russell will remove {parentCompanyName.Text} from the Russell Microcap Indexes effctive " +
                                                    $"{rDate} (after the close). {Additional_CA.Text}";


                    //insert CA story result
                    storyResultsBox.Text = corporateActionStory;
                }

            }

            else //get parent if not delist case
            {
                GetStockInputs(ref parent, true);
                GetStockInputs(ref child, false);
            } //End of Delist Case

            //check Parent growth/float/ dynamic for  US/ R3 member
            if ((parent.SizeDimension != sizeDimension.Other) && (parent.SizeDimension != sizeDimension.Global))
            { //us Member

                if ((parentGrowth.Text == "")&&(tenderOfferButton.IsChecked == false))
                { //check Growth rate for U.S parent company
                    MessageBox.Show("Please enter the Growth rate number for the U.S.member parent company");

                    return;

                }
                else
                {
                    if ((parent.SizeDimension != sizeDimension.MIC1)&&(tenderOfferButton.IsChecked ==false))
                    { //R3000

                        if (parentDynamic.Text == "")
                        {
                            MessageBox.Show("Please enter the Dynamic number for the R3000 parent company");
                            return;

                        }//edn check R3000 dynamic

                    }
                }//end of check US parent growth

                
            }//end check parent float and dynamice for US/R3

            //Check Child gowth.float/ dynamic for US/R3 member
            if ((child.SizeDimension != sizeDimension.Other) && (child.SizeDimension != sizeDimension.Global))
            { //us Member

                if ((childGrowth.Text == "")&&(tenderOfferButton.IsChecked == false))
                { //check Growth rate for U.S child company
                    MessageBox.Show("Please enter the Growth rate number for the U.S.member parent company");

                    return;

                }
                else
                {
                    if (child.SizeDimension != sizeDimension.MIC1)
                    { //R3000

                        if ((childDynamic.Text == "")&&(tenderOfferButton.IsChecked == false))
                        {
                            MessageBox.Show("Please enter the Dynamic number for the R3000 parent company");
                            return;

                        }//edn check R3000 dynamic

                    }
                }//end of check US child growth


            }//end check child float and dynamice for US/R3



            //Get all the inputs and check them
            bool isCash = (!String.IsNullOrWhiteSpace(cashTermsAmount.Text)||(cashTermsAmount.Text=="0"));
            bool isStock = (!String.IsNullOrWhiteSpace(stockTermsAmount.Text)||(stockTermsAmount.Text =="0"));

            //Find Cash and/orStock
            AndOrSymbol andOr = AndOrSymbol.neither;
            if (cashAndStock.IsChecked == true)
                andOr = AndOrSymbol.and;
            else if (cashOrStock.IsChecked == true)
                andOr = AndOrSymbol.or;
            else if (cashOrStock.IsChecked == false && cashAndStock.IsChecked == false)
                andOr = AndOrSymbol.neither;
            else
                MessageBox.Show("You should not see this message");


            //add switch statement for Radio Button selection
            if (mergerButton.IsChecked == true)
            {
                string ApprovalPending = "";
                if (OTCCheck.IsChecked == true)
                { ApprovalPending = "was approved at"; }
                else
                { ApprovalPending = "is pending"; }

                /////////////////////////////////////////////
                /// calculating newGrowth/newValue/NewDefensive/newDynamic
                long parentAvailable = FloatSharesCalculator(parent.ShareNum, parent.FloatNum);
                long childAvailable = FloatSharesCalculator(child.ShareNum, child.FloatNum);
                long newShares = MergerShareCalculator(parentAvailable, childAvailable, parent.StockTerms, parent.StockProrationNum);
                double newGrowth = 100 * Math.Round(ProbabilityCalculator(MergerShareCalculator(StyleShares(parentAvailable, parent.GrowthNum), childAvailable, parent.StockTerms, child.GrowthNum), newShares), 3);
                double newValue = 100 - newGrowth;
                double newDefensive = 100 * Math.Round(ProbabilityCalculator(MergerShareCalculator(StyleShares(parentAvailable, parent.DefensiveNum), childAvailable, parent.StockTerms, child.DefensiveNum), newShares), 3);
                double newDynamic = 100 - newDefensive;
                /////////////////////////////////////////////
                ///Stype change statement
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

                //////////////////////////////////////////////////////////////////////////////
                if ((isCash && !isStock)&&(OnlyCorS.IsChecked ==true))  //cash only
                {
                    MessageBox.Show("Stock Merger, Cash Only");

                    if ((parent.SizeDimension == sizeDimension.Other)||(parentSize.Text ==""))
                    {
                        //write story for parent non-member
             
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                            $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for ${cashTermsAmount.Text} in cash. The merger {ApprovalPending} {childCompanyName.Text}'s " +
                            $"shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else if (parent.SizeDimension == sizeDimension.Global)
                    {
                        //Write Global story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                            $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for ${cashTermsAmount.Text} in cash. The merger {ApprovalPending} {childCompanyName.Text}'s " +
                            $"shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else
                    {
                        //Write US story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                        $"{MembershipFinder(child)}): " +
                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                        $"{childCompanyName.Text} for cash. In the transaction, each share of {childCompanyName.Text} " +
                        $"will be exchanged for ${cashTermsAmount.Text} in cash. The merger {ApprovalPending} {childCompanyName.Text}'s " +
                        $"shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the " +
                        $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;

                    }
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                else if ((!isCash && isStock)&&(OnlyCorS.IsChecked ==true)) //stock only
                {
                    MessageBox.Show("Stock Merger, Stock Only");



                    if (parent.SizeDimension == sizeDimension.Other)
                    {
                        //write story for acquired by non-member
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) " +
                            $"will acquire {childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {stockTermsAmount.Text} of a share of {parentCompanyName.Text}. " +
                            $"The merger {ApprovalPending} {childCompanyName.Text}'s shareholder meeting on {pDate}. " +
                            $"{childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }

                    else if (parent.SizeDimension == sizeDimension.Global)
                    {
                        //Write acquired by  Global story
                        string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                            $"{MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                            $"{childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {stockTermsAmount.Text} of a share of {parentCompanyName.Text}. " +
                            $"The merger {ApprovalPending} {childCompanyName.Text}'s shareholder meeting on {pDate}. " +
                            $"{childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }

                    else
                    {
                        //Write US-us story

                        if (child.SizeDimension == sizeDimension.Global)
                        { //Parent US - Child Global stock only
                            string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $" {MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; " +
                            $"{MembershipFinder(parent)}" +
                            $") will acquire {childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {stockTermsAmount.Text} of a share of {parentCompanyName.Text}. The merger {ApprovalPending} {childCompanyName.Text}'s " +
                            $"shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger. Based on Russell's current projections, the new share total for {parentCompanyName.Text}" +
                            $" will be {newShares,0:N0} shares. The style probabilities are expected " +
                            $"to remain constant at {parent.GrowthNum * 100}% growth and {(1-parent.GrowthNum) * 100}% value and the stability " +
                            $"probabilities are expected to remain constant at {(1-parent.DynamicNum) * 100}% defensive and {parent.DynamicNum * 100}% dynamic.";
                            storyResultsBox.Text = corporateActionStory;


                        } //End Parent US - Child Global stock only
                        else
                        { //Parent US -Child US Stock
                            string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                            $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}" +
                            $" {MembershipFinder(child)}): " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; " +
                            $"{MembershipFinder(parent)}" +
                            $") will acquire {childCompanyName.Text} for stock. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for {stockTermsAmount.Text} of a share of {parentCompanyName.Text}. The merger {ApprovalPending} {childCompanyName.Text}'s " +
                            $"shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger. Based on Russell's current projections, the new share total for {parentCompanyName.Text}" +
                            $" will be {newShares,0:N0} shares. The style probabilities are expected " +
                            $"to {styleChange} {newGrowth}% growth and {newValue}% value and the stability " +
                            $"probabilities are expected to {stabChange} {newDefensive}% defensive and {newDynamic}% dynamic.";
                                storyResultsBox.Text = corporateActionStory; 
                        } //end Parent US -Child US stock
                    }

                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                else if ((cashOrStock.IsChecked ==true)||(cashAndStock.IsChecked == true)) //cash and stock
                {
                    switch (andOr)
                    {
                        case AndOrSymbol.and:
                            MessageBox.Show("Stock and Cash Merger");

                            if ((string.IsNullOrEmpty(stockTermsAmount.Text)) || (string.IsNullOrEmpty(cashTermsAmount.Text)))
                            {
                                MessageBox.Show("For Cash & Stock Merger, both cash term and stock term are needed.");
                                return;
                            }

                            if ((parent.SizeDimension == sizeDimension.Other) || (parentSize.Text == ""))
                            {
                                //write story for non-member
                                string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                    $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                    $"{MembershipFinder(child)}): " +
                                    $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                    $"{childCompanyName.Text} for cash and stock. In the transaction, each share of {childCompanyName.Text} will be exchanged for " +
                                    $"${cashTermsAmount.Text} in cash and {stockTermsAmount.Text} of a share of {parentCompanyName.Text} stock. The Merger {ApprovalPending} {childCompanyName.Text}'s" +
                                    $" shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the Russell Indexes upon completion of the merger.";

                                storyResultsBox.Text = corporateActionStory;

                            }//end non member

                            else if (parent.SizeDimension == sizeDimension.Global)
                            {
                                //Write Global story
                                string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                    $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                    $"{MembershipFinder(child)}): " + 
                                    $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                    $"{childCompanyName.Text} for cash and stock. In the transaction, each share of {childCompanyName.Text} will be exchanged for " +
                                    $"${cashTermsAmount.Text} in cash and {stockTermsAmount.Text} of a share of {parentCompanyName.Text} stock. The Merger {ApprovalPending} {childCompanyName.Text}'s" +
                                    $" shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the Russell Indexes upon completion of the merger." +
                                    $" Based on Russell's current projections, the new share total for {parentCompanyName.Text}" +
                                    $" will be {newShares,0:N0} shares. The style probabilities are expected " +
                                    $"to {styleChange} {newGrowth}% growth and {newValue}% value and the stability " +
                                    $"probabilities are expected to {stabChange} {newDefensive}% defensive and {newDynamic}% dynamic.";
               

                                storyResultsBox.Text = corporateActionStory;
                            }//End of Global

                            else
                            {
                                //Write US story

                                if (child.SizeDimension == sizeDimension.Global)
                                { //US Parent - Global child cashAndStock
                                    string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                      $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                      $"{MembershipFinder(child)}): " +
                                      $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                      $"{childCompanyName.Text} for stock and cash. In the transaction, each share of {childCompanyName.Text} will be exchanged for ${cashTermsAmount.Text} in cash and {stockTermsAmount.Text} " +
                                      $"of a share of {parentCompanyName.Text} stock. The merger {ApprovalPending} {childCompanyName.Text}'s " +
                                          $"shareholder meeting on {pDate}. {childCompanyName.Text} will be removed from the " +
                                          $"Russell Indexes upon completion of the merger. Based on Russell's current projections, the new share total for {parentCompanyName.Text}" +
                                          $" will be {newShares,0:N0} shares. The style probabilities are expected " +
                                          $"to remain constant at {parent.GrowthNum * 100}% growth and {(1 - parent.GrowthNum) * 100}% value and the stability " +
                                          $"probabilities are expected to remain constant at {(1 - parent.DynamicNum) * 100}% defensive and {parent.DynamicNum * 100}% dynamic.";
                                      
                                    storyResultsBox.Text = corporateActionStory;

                                }// End US Parent - Global Child cashAndStock

                                else
                                { //US Parent - US Child  CashAndStock
                                    string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                        $"{MembershipFinder(child)}): " +
                                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                        $"{childCompanyName.Text} for stock and cash. In the transaction, each share of {childCompanyName.Text} will be exchanged for ${cashTermsAmount.Text} in cash and {stockTermsAmount.Text} " +
                                        $"of a share of {parentCompanyName.Text} stock. The merger {ApprovalPending} {childCompanyName.Text}'s shareholder meeting on {pDate}. " +
                                        $"{childCompanyName.Text} will be removed from the Russell Indexes upon completion of the merger." +
                                        $" Based on Russell's current projections, the new share total for {parentCompanyName.Text}" +
                                        $" will be {newShares,0:N0} shares. The style probabilities are expected " +
                                        $"to {styleChange} {newGrowth}% growth and {newValue}% value and the stability " +
                                        $"probabilities are expected to {stabChange} {newDefensive}% defensive and {newDynamic}% dynamic.";

                                    storyResultsBox.Text = corporateActionStory; 
                                
                                }//End of US parent - US Child cashAndStock
                            }//end of US memeber

                            break;
                        case AndOrSymbol.or: //stock or Cash
                            MessageBox.Show("Stock or Cash Merger");

                            



                            if ((parent.SizeDimension == sizeDimension.Other) || (parentSize.Text == ""))
                            {
                                //write story for non-member
                                string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                        $"{MembershipFinder(child)}): " +
                                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                        $"{childCompanyName.Text} for stock or cash. In the transaction, each share of {childCompanyName.Text} will be exchaned for " +
                                        $"${cashTermsAmount.Text} in cash or {stockTermsAmount.Text} of a share of {parentCompanyName.Text} stock, subject to proration of approximately" +
                                        $" {parent.CashProrationNum * 100}% cash and {parent.StockProrationNum * 100}% stock." +
                                        $" The merger {ApprovalPending} {childCompanyName.Text}'s share holder meeting on {pDate}. {childCompanyName.Text} will be removed from the Russell Indexes upon completion of the merger.";

                                storyResultsBox.Text = corporateActionStory;

                            }//end non member

                            else if (parent.SizeDimension == sizeDimension.Global)
                            {
                                //Write Global story
                                string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                        $"{MembershipFinder(child)}): " +
                                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                        $"{childCompanyName.Text} for stock or cash. In the transaction, each share of {childCompanyName.Text} will be exchaned for " +
                                        $"${cashTermsAmount.Text} in cash or {stockTermsAmount.Text} of a share of {parentCompanyName.Text} stock, subject to proration of approximately" +
                                        $" {parent.CashProrationNum * 100}% cash and {parent.StockProrationNum * 100}% stock." +
                                        $" The merger {ApprovalPending} {childCompanyName.Text}'s share holder meeting on {pDate}. {childCompanyName.Text} will be removed from the Russell Indexes upon completion of the merger.";


                                storyResultsBox.Text = corporateActionStory;
                            }//End of Global

                            else
                            {
                                //Write US story
                                if (child.SizeDimension == sizeDimension.Global)
                                { //US parent - Global parent cashOrStock 
                                    string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                        $"{MembershipFinder(child)}): " +
                                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                        $"{childCompanyName.Text} for stock or cash. In the transaction, each share of {childCompanyName.Text} will be exchaned for " +
                                        $"${cashTermsAmount.Text} in cash or {stockTermsAmount.Text} of a share of {parentCompanyName.Text} stock, subject to proration of approximately" +
                                        $" {parent.CashProrationNum * 100}% cash and {parent.StockProrationNum * 100}% stock. The new share total for {parentCompanyName.Text} is projected to be {newShares,0:N0}." +
                                        $" The style probabilities will {styleChange} {parent.GrowthNum * 100}% growth and {(1-parent.GrowthNum) * 100}% value and the stability " +
                                        $"probabilities are expected to {stabChange} {(1-parent.DynamicNum) * 100}% defensive and {parent.DynamicNum * 100}% dynamic. " +
                                        $" The merger {ApprovalPending} {childCompanyName.Text}'s share holder meeting on {pDate}. {childCompanyName.Text} will be removed from the Russell Indexes upon completion of the merger.";


                                    storyResultsBox.Text = corporateActionStory;

                                }//end of US Parent - Global Child cashOrStock
                                else

                                { //US parent - US child cashOrStock
                                    string corporateActionStory = $"Projected to close {closeDate.Text}:" +
                                        $" {childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                                        $"{MembershipFinder(child)}): " +
                                        $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                                        $"{childCompanyName.Text} for stock or cash. In the transaction, each share of {childCompanyName.Text} will be exchaned for " +
                                        $"${cashTermsAmount.Text} in cash or {stockTermsAmount.Text} of a share of {parentCompanyName.Text} stock, subject to proration of approximately" +
                                        $" {parent.CashProrationNum * 100}% cash and {parent.StockProrationNum * 100}% stock. The new share total for {parentCompanyName.Text} is projected to be {newShares,0:N0}." +
                                        $" The style probabilities will {styleChange} {newGrowth}% growth and {newValue}% value and the stability " +
                                        $"probabilities are expected to {stabChange} {newDefensive}% defensive and {newDynamic}% dynamic. " +
                                        $" The merger {ApprovalPending} {childCompanyName.Text}'s share holder meeting on {pDate}. {childCompanyName.Text} will be removed from the Russell Indexes upon completion of the merger.";


                                    storyResultsBox.Text = corporateActionStory;

                                }//end of US parent-US child cashOrStock 


                            }//end of US memeber


                            break;
                        case AndOrSymbol.neither:
                            MessageBox.Show("Please select whether it is stock and cash or stock or cash.");
                            break;
                        default:
                            break;
                    }//end switch  andOr

 

                }
         

            }
            //End of Merger
     /***************************************************************************************************************************************************************************************/
            else if (spinOffButton.IsChecked == true)  //spin off Story
            {

                //Write a spin off story

                
                string childMember;
                string membershipStatement="";

                //get all memberships official names 
                string[] wholeMembership = MembershipFinder(parent).Split(',');
                string printWMembership="";

                //get the first index name from the list 
                if (wholeMembership[0] == "R1")
                { wholeMembership[0] = "Russell 1000"; }
                else if (wholeMembership[0] == "T2")
                { wholeMembership[0] = "Russell Top 200"; }
                else if (wholeMembership[0] == "R2")
                { wholeMembership[0] = "Russell 2000"; }
                else if (wholeMembership[0] == "R25")
                { wholeMembership[0] = "Russell 25000"; }
                else if (wholeMembership[0] == "MC")
                { wholeMembership[0] = "Russell Mid Cap"; }

                //get the rest of membership name
                for (int i = 1; i < wholeMembership.Length; i++)
                {

                    if (wholeMembership[i] == "R1")
                     { wholeMembership[i] = ", Russell 1000"; }
                     else if (wholeMembership[i] == " T2")
                     { wholeMembership[i] = ", Russell Top 200"; }
                     else if (wholeMembership[i] == " R2")
                     { wholeMembership[i] = ", Russell 2000"; }
                     else if (wholeMembership[i] == " R25")
                     { wholeMembership[i] = ", Russell 2500"; }
                     else if (wholeMembership[i] == " MC")
                     { wholeMembership[i] = ", Russell Midcap"; }
                     else
                     {   //remove non membership element from array
                     
                         wholeMembership = wholeMembership.Where((val,idx) =>idx !=i).ToArray();
                        i = i - 1;
                     }           

                }
               
                    foreach (string memeber in wholeMembership)
                    {
                    printWMembership = printWMembership + memeber;
                    } //end of get whole member name


                if (string.IsNullOrEmpty(stockTermsAmount.Text)) //check stock term --- close if it's null
                {
                    MessageBox.Show("Please Enter the stock Term of Spin Off Corperate Action.");
                    return;
                }//end check stock term.

                if (MembershipFinder(parent).Contains("RS") == true) //check if parent is RSCC
                {
         
                        childMember = $"{MembershipFinder(parent)} ";
                        membershipStatement = printWMembership + " and Russell Small Cap Completeness). ";
                }
                else //if parent is not RSCC 
                {
                    if (childSP5.IsChecked == true) // if child is RSCC
                    {
                        childMember = $"{MembershipFinder(parent)}, RS";
                        membershipStatement = printWMembership + ") and Russell Small Cap Completeness. ";
                    }

                    else //if child is not RSCC
                    {
                        childMember = MembershipFinder(parent);
                        membershipStatement = printWMembership + ").";
                    }

                } //end of get child membership

                //style statement
                string styleStatment = "";

                //check parentdynamic  
                if (string.IsNullOrEmpty(parentDynamic.Text))
                {
                    styleStatment = $"{parent.GrowthNum * 100}% growth and {parent.ValueNum * 100}% value.";
                }
                else
                {
                    styleStatment = $"{parent.GrowthNum * 100}% growth and {parent.ValueNum * 100}% value and the stability probabilities will be " +
                                    $"{parent.DefensiveNum * 100}% defensive and {parent.DynamicNum * 100}% dynamic.";
                } //end of styleStatment

                string nShare = "shares";
                if (stockTermsAmount.Text =="1")
                {
                    nShare = "share";
                }

                string corporateActionStory = $"Projected to close {rDate}:" +
                        $" {childCompanyName.Text} {childNameSuffix.Text} ({childTicker.Text}; {childMember});" +
                        $" {parentCompanyName.Text} {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will spin off its shares of {childCompanyName.Text}." +
                        $" In the transaction, holders of {parentCompanyName.Text} will receive {stockTermsAmount.Text} {nShare} of {childCompanyName.Text} for every share of {parentCompanyName.Text}. " +
                        $"{childCompanyName.Text} is projected to be added to Russell Indexes as it satisfies the Russell’s inclusion criteria. The new company will be added to " +
                        $"{parentCompanyName.Text}'s respective indexes ({membershipStatement} " +
                        $"Based on {parentCompanyName.Text}'s current probabilities, {childCompanyName.Text}'s style probabilities will be " +
                        $"{styleStatment} " +
                        $"The share total for {childCompanyName.Text} is projected to {child.ShareNum, 0:N0}. {childCompanyName.Text} shares will be distributed to shareholders on {pDate} (after the close)." +
                        $" The price for {parentCompanyName.Text} will be adjusted to reflect the spin-off. {Additional_CA.Text}";


                storyResultsBox.Text = corporateActionStory; 


            }// end of Spin off
      
        /************************************************************************************************************************************************************************************/               
            else if (tenderOfferButton.IsChecked == true)
            {
                //write a tender offer story


                string corporateActionStory = $"{childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                    $"{MembershipFinder(child)}); " +
                    $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                    $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                    $"will be exchanged for ${cashTermsAmount.Text} in cash. The offer is set to expire on " +
                    $"{pDate}. {childCompanyName.Text} will be removed from the " +
                    $"Russell Indexes upon completion of the merger.";
                storyResultsBox.Text = corporateActionStory;


                /*    if (parent.SizeDimension == sizeDimension.Other)
                    {

                        //write story for acquired by non-member

                        string corporateActionStory = $"{childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                            $"{MembershipFinder(child)}); " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                            $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for ${cashTermsAmount.Text} in cash. The offer is set to expire on " +
                            $"{pDate}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;
                    }
                    else if (parent.SizeDimension == sizeDimension.Global)
                    {
                        //Write acquired by  Global story

                        string corporateActionStory = $"{childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                            $"{MembershipFinder(child)}); " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                            $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for ${cashTermsAmount.Text} in cash. The offer is set to expire on " +
                            $"{pDate}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;

                    }
                    else
                    {
                        //Write US-us story

                        string corporateActionStory = $"{childCompanyName.Text}, {childNameSuffix.Text} ({childTicker.Text}; " +
                            $"{MembershipFinder(child)}); " +
                            $"{parentCompanyName.Text}, {parentNameSuffix.Text} ({parentTicker.Text}; {MembershipFinder(parent)}) will acquire " +
                            $"{childCompanyName.Text} through a cash tender offer. In the transaction, each share of {childCompanyName.Text} " +
                            $"will be exchanged for ${cashTermsAmount.Text} in cash. The offer is set to expire on " +
                            $"{pDate}. {childCompanyName.Text} will be removed from the " +
                            $"Russell Indexes upon completion of the merger.";
                        storyResultsBox.Text = corporateActionStory;

                    }*/



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
            long probabilityShares = Convert.ToInt64(shares * probability);
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
                    return "Russell Microcap Index Member";
                case sizeDimension.Other:
                    return "not in the Russell Indexes";
                case sizeDimension.Global:
                    return "FTSE Russell Universe Member";
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
                    if (Convert.ToDouble(Input) <= 1)
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
                try
                { input.ShareNum = Convert.ToInt64(((MainWindow)Application.Current.MainWindow).parentTSO.Text); }
                catch (FormatException)
                {
                    input.ShareNum = 0000000000;

                }
                //when dynamic is not null 
                if (!String.IsNullOrEmpty (((MainWindow)Application.Current.MainWindow).parentDynamic.Text))
                {
                    input.DynamicNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentDynamic.Text);
                    input.DefensiveNum = (1 - (ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentDynamic.Text)));
                    input.IsDefensive = !input.DefensiveNum.Equals(0.00);
                    input.IsDynamic = !input.DynamicNum.Equals(0.00);
                }//end of check parent dynamic and defense 
               
                input.FloatNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentFloat.Text);

                if (!String.IsNullOrEmpty(((MainWindow)Application.Current.MainWindow).parentGrowth.Text))
                {
                    input.GrowthNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentGrowth.Text);
                    input.ValueNum = (1 - (ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentGrowth.Text)));
                    input.IsGrowth = !input.GrowthNum.Equals(0.00);
                    input.IsValue = !input.ValueNum.Equals(0.00);
                }

                else
                {
              
                   
                        input.IsGrowth = false;
                        input.IsValue = false;
                    
                 }



                if (((MainWindow)Application.Current.MainWindow).mergerButton.IsChecked == true)
                {
                    try
                    {
                        input.StockProrationNum = Convert.ToDouble(((MainWindow)Application.Current.MainWindow).stockProration.Text);
                        input.CashProrationNum = Convert.ToDouble(((MainWindow)Application.Current.MainWindow).cashProration.Text);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Please Check the Stock Proration or Cash Proration.");
                    }
                }

                if (((MainWindow)Application.Current.MainWindow).parentSP5.IsChecked == true)
                {
                    input.RSCC = true;
                }

                try
                {
                    input.StockTerms = Convert.ToDouble(((MainWindow)Application.Current.MainWindow).stockTermsAmount.Text);
                }
                catch (Exception)
                {
                    input.StockTerms = 0.000;
                }
                /////////
                if ((((MainWindow)Application.Current.MainWindow).mergerButton.IsChecked == true) && ((ConvertToDouble(((MainWindow)Application.Current.MainWindow).stockProration.Text)+ ConvertToDouble(((MainWindow)Application.Current.MainWindow).cashProration.Text))<1))
                { input.StockProrationNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).stockProration.Text);
                    input.CashProrationNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).cashProration.Text);
                }
                    //////////

                try
                {
                    input.SizeDimension = (sizeDimension)Enum.Parse(typeof(sizeDimension), ((MainWindow)Application.Current.MainWindow).parentSize.Text);
                }
                catch (Exception)
                {
                    input.SizeDimension = sizeDimension.Other;
                }
            }
            else//child Input
            {
                try
                {
                    input.SizeDimension = (sizeDimension)Enum.Parse(typeof(sizeDimension), ((MainWindow)Application.Current.MainWindow).childSize.Text);
                }
                catch (Exception)
                {
                    input.SizeDimension = sizeDimension.Other;
                }

                if (input.SizeDimension == sizeDimension.Global)
                {//Global Child's growth and value rate = parent's 
                    input.GrowthNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentGrowth.Text);
                    input.ValueNum = (1 - (ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentGrowth.Text)));
                }//end of setting Global Child's growth and value rate = parent's 
                else if (!string.IsNullOrEmpty((((MainWindow)Application.Current.MainWindow).childGrowth.Text)))
                { //get growth and value rate for only US child 
                    input.GrowthNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).childGrowth.Text);
                    input.ValueNum = (1 - (ConvertToDouble(((MainWindow)Application.Current.MainWindow).childGrowth.Text)));
                }//end of getting Growth and Value for non Global


                input.ShareNum = Convert.ToInt64(((MainWindow)Application.Current.MainWindow).childTSO.Text);

                //check child dynamic
                if (!String.IsNullOrEmpty(((MainWindow)Application.Current.MainWindow).childDynamic.Text))
                {
                    input.DynamicNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).childDynamic.Text);
                    input.DefensiveNum = (1 - (ConvertToDouble(((MainWindow)Application.Current.MainWindow).childDynamic.Text)));
                }

                //get child float
                //In Spin off, parent float = child float
                if (((MainWindow)Application.Current.MainWindow).spinOffButton.IsChecked == true)
                {
                    input.FloatNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).parentFloat.Text);
                    input.ShareNum = Convert.ToInt64(Math.Floor(Convert.ToInt64(((MainWindow)Application.Current.MainWindow).parentTSO.Text) * Convert.ToDouble(((MainWindow)Application.Current.MainWindow).parentFloat.Text)) * Convert.ToDouble(((MainWindow)Application.Current.MainWindow).stockTermsAmount.Text));


                }
                else //else child float =  itself.
                { 
                    input.FloatNum = ConvertToDouble(((MainWindow)Application.Current.MainWindow).childFloat.Text); 
                }//end get child float num

                if (!String.IsNullOrEmpty(((MainWindow)Application.Current.MainWindow).childGrowth.Text))
                {
                    input.IsGrowth = !input.GrowthNum.Equals(0.00);
                    input.IsValue = !input.ValueNum.Equals(0.00);
                }
                else
                {
                    input.IsGrowth = false;
                    input.IsValue = false;
                }

                if (!String.IsNullOrEmpty(((MainWindow)Application.Current.MainWindow).childDynamic.Text))
                {
                    input.IsDefensive = !input.DefensiveNum.Equals(0.00);
                    input.IsDynamic = !input.DynamicNum.Equals(0.00);
                }
                else 
                {
                    input.IsDefensive = false;
                    input.IsDynamic = false;
                }

                if (((MainWindow)Application.Current.MainWindow).childSP5.IsChecked == true)
                {
                    input.RSCC = true;
                }


            }
            
            return input;
        }

        private void resetAll_Click(object sender, RoutedEventArgs e)
        {
            //reset parent
            ((MainWindow)Application.Current.MainWindow).parentCompanyName.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentTicker.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentTSO.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentFloat.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentSize.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentGrowth.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentDynamic.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentSP5.IsChecked = false;
            ((MainWindow)Application.Current.MainWindow).parentNameSuffix.Text = "";
            ((MainWindow)Application.Current.MainWindow).parentExchange.Text = "";
            ((MainWindow)Application.Current.MainWindow).ParentR3.IsChecked = false;


           //reset child
           ((MainWindow)Application.Current.MainWindow).childCompanyName.Text = "";
            ((MainWindow)Application.Current.MainWindow).childTicker.Text = "";
            ((MainWindow)Application.Current.MainWindow).childTSO.Text = "";
            ((MainWindow)Application.Current.MainWindow).childFloat.Text = "";
            ((MainWindow)Application.Current.MainWindow).childSize.Text = "";
            ((MainWindow)Application.Current.MainWindow).childGrowth.Text = "";
            ((MainWindow)Application.Current.MainWindow).childDynamic.Text = "";
            ((MainWindow)Application.Current.MainWindow).childSP5.IsChecked = false;
            ((MainWindow)Application.Current.MainWindow).childNameSuffix.Text = "";
            ((MainWindow)Application.Current.MainWindow).ChildExchange.Text = "";
            ((MainWindow)Application.Current.MainWindow).ChildR3.IsChecked = false;
            ((MainWindow)Application.Current.MainWindow).ChildExchange.IsEnabled = true;

            //reset Term/amount/proration
            ((MainWindow)Application.Current.MainWindow).stockProration.Text  = "";
            ((MainWindow)Application.Current.MainWindow).cashProration.Text  = "";
            ((MainWindow)Application.Current.MainWindow).cashTermsAmount.Text = "";
            ((MainWindow)Application.Current.MainWindow).stockTermsAmount.Text = "";
            ((MainWindow)Application.Current.MainWindow).meetingDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).closeDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).OTCCheck.IsChecked = false;
            ((MainWindow)Application.Current.MainWindow).cashAndStock.IsChecked = false;
            ((MainWindow)Application.Current.MainWindow).cashOrStock.IsChecked = false;
            ((MainWindow)Application.Current.MainWindow).OnlyCorS.IsChecked = false;


            //reset inputs
            



            storyResultsBox.Text = "Corporate Action Story";
       } //end of rest button 

        private void quitApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mergerButton_Click(object sender, RoutedEventArgs e)
        {
            
            //reset parent
            ((MainWindow)Application.Current.MainWindow).companyNameParent.Text = "Parent Company";


            //reset child
            ((MainWindow)Application.Current.MainWindow).companyNameChild.Text = "Child Company";


            //enable the child company textbox fileds
            ((MainWindow)Application.Current.MainWindow).childCompanyName.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childTicker.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childTSO.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childFloat.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childSize.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childGrowth.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childDynamic.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childSP5.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childNameSuffix.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).ChildExchange.IsEnabled = true;

            //reset Term/amount/proration

            ((MainWindow)Application.Current.MainWindow).meetingDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).closeDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).meetingDateLable.Text = "Meeting Date";
            ((MainWindow)Application.Current.MainWindow).closeDateLabel.Text = "Projected to Close";
            ((MainWindow)Application.Current.MainWindow).OTCCheck.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).OTCCheck.Content = "Approval";
            ((MainWindow)Application.Current.MainWindow).cashTermsAmount.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).cashProration.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).stockProration.IsEnabled = true;

            ((MainWindow)Application.Current.MainWindow).cashAndStock.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).cashOrStock.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).OnlyCorS.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).meetingDate.IsEnabled = true; 
            ((MainWindow)Application.Current.MainWindow).stockTermsAmount.IsEnabled = true;






        }


        private void tenderOfferButton_Click(object sender, RoutedEventArgs e)
        {
            //reset parent
            ((MainWindow)Application.Current.MainWindow).companyNameParent.Text = "Parent Company";


            //reset child
            ((MainWindow)Application.Current.MainWindow).companyNameChild.Text = "Child Company";


            //enable the child company textbox fileds
            ((MainWindow)Application.Current.MainWindow).childCompanyName.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childTicker.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childTSO.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childFloat.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childSize.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childGrowth.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childDynamic.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childSP5.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childNameSuffix.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).ChildExchange.IsEnabled = true;

            //reset Term/amount/proration

            ((MainWindow)Application.Current.MainWindow).meetingDateLable.Text = "";
            ((MainWindow)Application.Current.MainWindow).closeDateLabel.Text = "  Expiration  Date";
            ((MainWindow)Application.Current.MainWindow).OTCCheck.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).OTCCheck.Content = "";
            ((MainWindow)Application.Current.MainWindow).cashTermsAmount.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).cashProration.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).closeDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).meetingDate.Text = "";

            ((MainWindow)Application.Current.MainWindow).cashAndStock.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).cashOrStock.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).OnlyCorS.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).meetingDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).meetingDate.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).stockProration.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).stockTermsAmount.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).cashProration.IsEnabled = false;
             


        }

        private void delistingButton_Click(object sender, RoutedEventArgs e)
        {
            //reset parent
            ((MainWindow)Application.Current.MainWindow).companyNameParent.Text = "Delisted Company";


            //reset child
            ((MainWindow)Application.Current.MainWindow).companyNameChild.Text = "";


            //disable the child company textbox fileds
            ((MainWindow)Application.Current.MainWindow).childCompanyName.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childTicker.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childTSO.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childFloat.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childSize.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childGrowth.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childDynamic.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childSP5.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).childNameSuffix.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).ChildExchange.IsEnabled = false;
            //reset Term/amount/proration

            ((MainWindow)Application.Current.MainWindow).meetingDateLable.Text = "Delisting Date";
            ((MainWindow)Application.Current.MainWindow).closeDateLabel.Text = "The  Removal  Date";
            ((MainWindow)Application.Current.MainWindow).OTCCheck.IsEnabled  = true;
            ((MainWindow)Application.Current.MainWindow).OTCCheck.Content  = "Not in OTC";
            ((MainWindow)Application.Current.MainWindow).cashTermsAmount.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).cashProration.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).closeDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).meetingDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).meetingDate.IsEnabled = true;



            ((MainWindow)Application.Current.MainWindow).cashAndStock.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).cashOrStock.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).OnlyCorS.IsEnabled = false;
            


        }

        private void spinOffButton_Click(object sender, RoutedEventArgs e)
        {
            //reset parent
            ((MainWindow)Application.Current.MainWindow).companyNameParent.Text = "Parent Company";

            //reset child
            ((MainWindow)Application.Current.MainWindow).companyNameChild.Text = "Child Company";


            //enable the child company textbox fileds
            ((MainWindow)Application.Current.MainWindow).childCompanyName.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childTicker.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childTSO.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childFloat.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childSize.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childGrowth.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childDynamic.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childSP5.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).childNameSuffix.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).ChildExchange.IsEnabled = true;

            //reset Term/amount/proration

            ((MainWindow)Application.Current.MainWindow).cashTermsAmount.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).cashProration.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).meetingDateLable.Text = "Distribute Date";
            ((MainWindow)Application.Current.MainWindow).closeDateLabel.Text = "Projected  to  Close";
            ((MainWindow)Application.Current.MainWindow).OTCCheck.IsEnabled = false; 
            ((MainWindow)Application.Current.MainWindow).OTCCheck.Content = "";
            ((MainWindow)Application.Current.MainWindow).closeDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).meetingDate.Text = "";
            ((MainWindow)Application.Current.MainWindow).stockProration.IsEnabled = false;


            ((MainWindow)Application.Current.MainWindow).cashAndStock.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).cashOrStock.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).OnlyCorS.IsEnabled = false;
            ((MainWindow)Application.Current.MainWindow).meetingDate.IsEnabled = true;
            ((MainWindow)Application.Current.MainWindow).stockTermsAmount.IsEnabled = true;



        }

        private void cashTermsAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((cashOrStock.IsChecked == false) && (mergerButton.IsChecked == true)) //when cashOrStock check and text change cashTermsAmount
            {//non cash or stock cashTerms
                if (cashAndStock.IsChecked != true)

                {
                    if (!String.IsNullOrEmpty(cashTermsAmount.Text))//
                    {
                        if (cashTermsAmount.Text == "0")
                        {
                            cashProration.Text = "0";
                            cashProration.IsEnabled = false;
                            stockProration.IsEnabled = false;
                        }
                        else
                        {
                            cashProration.Text = "1";
                            stockProration.Text = "0";
                            cashProration.IsEnabled = false;
                            stockProration.IsEnabled = false;
                            stockTermsAmount.Text = "";
                        }
                    }
                    else if ((String.IsNullOrEmpty(cashTermsAmount.Text)))
                    {
                        cashProration.Text = "0";
                        cashProration.IsEnabled = false;
                        stockProration.IsEnabled = false;
                    }

                }//end of set non cashAndstock proration
                else
                {//set  cashAndStock propartion
                    cashProration.Text = "0";
                    stockProration.Text = "1";
                    cashProration.IsEnabled = false;
                    stockProration.IsEnabled = false;
                } //end of cash and stock proration set


            }//end of cashOrStock check and text change cashTermsAmount

        }//end cashTemsAmount_TextChanged

        private void stockTermsAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((cashOrStock.IsChecked == false) && (mergerButton.IsChecked == true)) //when cashOrStock check and text change stockTermsAmount
            { //non cashOrStock stockTermsAmount 

                if (cashAndStock.IsChecked != true)
                { //non cash and stock stockTermsAmount

                    if (!String.IsNullOrEmpty(stockTermsAmount.Text))//
                    {
                        if (stockTermsAmount.Text == "0")
                        {
                            stockProration.Text = "0";
                            cashProration.IsEnabled = false;
                            stockProration.IsEnabled = false;
                        }
                        else
                        {
                            cashProration.Text = "0";
                            stockProration.Text = "1";
                            cashProration.IsEnabled = false;
                            stockProration.IsEnabled = false;
                            cashTermsAmount.Text = "";
                        }

                    }
                    else if (String.IsNullOrEmpty(stockTermsAmount.Text))
                    {
                        stockProration.Text = "0";
                        cashProration.IsEnabled = false;
                        stockProration.IsEnabled = false;
                    }

                }//end non CashAndStock stockTerm set
                else
                {
                    //set cashAndStock stockTermAmount
                    cashProration.Text = "0";
                    stockProration.Text = "1";
                    cashProration.IsEnabled = false;
                    stockProration.IsEnabled = false;
                }


            }//end of cashOrStock check and text change cashTermsAmount

        }// end of stockTermsAmount_TextChanged

        private void cashProration_TextChanged(object sender, TextChangedEventArgs e)
        { 
            if (((cashOrStock.IsChecked ==true)) && (mergerButton.IsChecked == true))//when cashAndStock check cashProration
            {
                if (!String.IsNullOrEmpty(cashProration.Text))
                {
                    double cProration;

                    try
                    {
                        cProration = Convert.ToDouble(cashProration.Text);
                    }

                    catch (FormatException)
                    {
                        MessageBox.Show("Please Make Sure you are inputing a double number start with a number");
                        return;
                    }
                    if ((0 <= cProration) && (cProration <= 1))
                    {
                        stockProration.Text = $"{(1 - cProration)}";
                    }//end check 

                    else
                    {
                        MessageBox.Show("Proration must be neither smaller than 0 nor larger then 1. Please re-enter your proration");
                        return;
                    }
                      
                }

            }//end of cashProration check
        }//end of  cashProration_TextChanged

        private void stockProration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((cashOrStock.IsChecked == true)) && (mergerButton.IsChecked == true))//when cashAndStock check cashProration
            {
                double sProration;
                if (!String.IsNullOrEmpty(stockProration.Text))
                {
                    try
                    {
                        sProration = Convert.ToDouble(stockProration.Text);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Please Make Sure you are inputing a double number start with a number");
                        return;
                    }
                
                    
                    if ((0 <= sProration) && (sProration <= 1))
                    {
                        cashProration.Text = $"{(1 - sProration)}";
                    }//end check 

                    else
                    {
                        MessageBox.Show("Proration must be neither smaller than 0 nor larger then 1. Please re-enter your proration");
                        return;
                    }

                }

            }//end of cashProration check




        }

        private void cashAndStock_Click(object sender, RoutedEventArgs e)
        {
            if (mergerButton.IsChecked == true)
            {
                stockProration.IsEnabled = false;
                cashProration.IsEnabled = false;
                OnlyCorS.IsChecked = false;
            }
            else 
            {
                stockProration.IsEnabled = false;
                cashProration.IsEnabled = false;
                OnlyCorS.IsChecked = false;
                MessageBox.Show("Please select Corporate Active Type First");

            }

        }

        private void cashOrStock_Click(object sender, RoutedEventArgs e)
        {
            if (mergerButton.IsChecked == true)

            {
                stockProration.IsEnabled = true;
                cashProration.IsEnabled = true;
                OnlyCorS.IsChecked = false;
            }
            else
            {
                stockProration.IsEnabled = false;
                cashProration.IsEnabled = false;
                OnlyCorS.IsChecked = false;
                MessageBox.Show("Please select Corporate Active Type First");
            }
        }

        private void OnlyCorS_Click(object sender, RoutedEventArgs e)
        {
            if (mergerButton.IsChecked == true)
            {
                cashProration.IsEnabled = false;
                stockProration.IsEnabled = false;
                cashOrStock.IsChecked = false;
                cashAndStock.IsChecked =false;
            }
            else
            {
                stockProration.IsEnabled = false;
                cashProration.IsEnabled = false;
                OnlyCorS.IsChecked = false;
                MessageBox.Show("Please select Corporate Active Type First");
            }
        }
    }

}

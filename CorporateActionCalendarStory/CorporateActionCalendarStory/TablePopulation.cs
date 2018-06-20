using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateActionCalendarStory
{
    public class TablePopulation
    {
        public string parentName { get; set; }
        public string parentTicker { get; set; }
        public string parentTSO { get; set; }
        public string parentFloat { get; set; }
        public string parentSize { get; set; }
        public string parentGrowth { get; set; }
        public string parentDynamic { get; set; }
        public string parentRSCC { get; set; }
        public string childName { get; set; }
        public string childTicker { get; set; }
        public string childTSO { get; set; }
        public string childFloat { get; set; }
        public string childSize { get; set; }
        public string childGrowth { get; set; }
        public string childDynamic { get; set; }
        public string childRSCC { get; set; }
        public TablePopulation()
        {

        }
        
    }
    public class StockInputs
    {
        //Get all the inputs and check them
        //bool isCash = !String.IsNullOrWhiteSpace(cashTermsAmount.Text);
        //bool isStock = !String.IsNullOrWhiteSpace(stockTermsAmount.Text);
        //take in probabilities and float
        public string Name { get; set; }
        public string Ticker { get; set; }
        public double GrowthNum { get; set; }//= ConvertToDouble(parentGrowth.Text);
        public double ValueNum { get; set; }//= 1 - ConvertToDouble(parentGrowth.Text);
        public double DynamicNum { get; set; }//= ConvertToDouble(parentDynamic.Text);
        public double DefensiveNum { get; set; }//= 1 - ConvertToDouble(parentDynamic.Text);
        public double FloatNum { get; set; }//= ConvertToDouble(parentFloat.Text);
        public bool RSCC { get; set; }//= CheckSP(parentSP5.IsChecked);
        public long ShareNum { get; set; }//= ConvertToLong(parentTSO.Text);
        public StockInputs()
        {

        }
    }

}

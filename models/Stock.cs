using System.Globalization;

namespace Inoa
{
    public class Stock
    {
        public string Name {get; private set; } = String.Empty;
        private decimal MinValue;
        private decimal MaxValue;

        public Stock(string assetName, string minString, string maxString)
        {
            Name = assetName.ToUpper();

            minString = minString.Replace(',', '.');
            if(!Decimal.TryParse(minString, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out MinValue))
                throw new Exception("Not a valid value for the minimum monitoring price");

            maxString = maxString.Replace(',', '.');
            if(!Decimal.TryParse(maxString, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out MaxValue))
               throw new Exception("Not a valid value for the maximum monitoring price");

            if(MaxValue <= MinValue)
                throw new Exception("Maximum value is lower than minimum value or they are the same. Maybe you should swap values");
        }

        public bool isLesser(decimal curValue)
        {
            return curValue < MinValue;
        }

        public bool isGreater(decimal curValue)
        {
            return curValue > MaxValue;
        }
    }
}
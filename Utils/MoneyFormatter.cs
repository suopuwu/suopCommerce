namespace SuopCommerce.Utils
{
    public static class MoneyFormatter
    {
        //adds extra decimals to a double, but not the dollar symbol
        public static string formatDouble (double d)
        {
            string returnString = d.ToString();
            string[] parts = returnString.Split ('.');
            if (parts.Length == 1) {
                return returnString + ".00";
            } else
            {
                while (parts[1].Length < 2) {
                    parts[1] += "0";
                }
                return parts[0] + "." + parts[1];

            }
        }
    }
}

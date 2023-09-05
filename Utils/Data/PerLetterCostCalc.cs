namespace SuopCommerce.Utils.Data
{
    public static class PerLetterCostCalc
    {
        public static double calc(int length, double cost, int freeLetters)
        {
            if (length <= freeLetters) return 0;
            return (length - freeLetters) * cost;
        }
    }
}

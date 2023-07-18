using System.IO;

namespace SuopCommerce.Models
{
    public static class Extras
    {
        public record Extra();
        public record PerLetter(string Title, string Id, double Cost): Extra;

        public record InvalidExtra(string message): Extra;
        public static Extra ParseExtra(string extraString)
        {
            void validateExtraSections(string[] parts, int length)
            {
                if (parts.Length != length) throw new Exception(parts.Length + " is not a valid number of arguments for " + parts[0]);
            }
            try
            {
                var parts = extraString.Split(":");

                switch (parts[0].ToLower())
                {//per letter:<Display text>:<id>:<Cost per letter>
                    case "per letter":
                        validateExtraSections(parts, 4);
                        return new PerLetter(parts[1], parts[2], double.Parse(parts[3]));
                    default:
                        throw new Exception(parts[0] + " is not a valid extra type");
                }
            }
            catch (Exception e)
            {
                return new InvalidExtra(e.Message);
            }
        }
    }
}

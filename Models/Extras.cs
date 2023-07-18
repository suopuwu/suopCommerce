using System.IO;

namespace SuopCommerce.Models
{
    public static class Extras
    {
        public enum Types
        {
            Invalid,
            PerLetter
        }

        public record Extra(
            Types Type = Types.Invalid,
            string Text = "",
            string Id = "",
            double Cost = 0
            );

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
                        return new Extra(Type: Types.PerLetter, Text: parts[1], Id: parts[2], Cost: Double.Parse(parts[3]));
                    default:
                        throw new Exception(parts[0] + " is not a valid extra type");
                }
            }
            catch (Exception e)
            {
                return new Extra(Type: Types.PerLetter, Text: e.Message);
            }
        }
    }
}

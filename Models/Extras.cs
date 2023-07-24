using suopCommerce.Models;
using System.IO;

namespace SuopCommerce.Models
{
    public static class Extras
    {
        public enum Types
        {
            Invalid,
            PerLetter,
            TextField,
        }

        public record Extra(
            Types Type = Types.Invalid,
            string Text = "",
            string Id = "",
            double Cost = 0,
            string HintText = ""
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
                {
                    case "per letter"://per letter:<id>:<Display text>:<Cost per letter>
                        validateExtraSections(parts, 4);
                        return new Extra(Type: Types.PerLetter, Id: parts[1], Text:parts[2], Cost: Double.Parse(parts[3]));
                    case "text field"://per letter:<id>:<Display text>:<Hint text>
                        validateExtraSections(parts, 4);
                        return new Extra(Type: Types.TextField, Id: parts[1], Text: parts[2], HintText: parts[3]);
                    default:
                        throw new Exception(parts[0] + " is not a valid extra type");
                }
            }
            catch (Exception e)
            {
                return new Extra(Type: Types.Invalid, Text: e.Message);
            }
        }

        public static Extra GetExtraFromIdInProduct(string id, Product product)
        {
            Dictionary<string, Extras.Extra> potentialExtras = new();
            foreach (var extraString in product.Extras)
            {
                //create a map with all valid extras, from the id of the extra to the extra itself
                var extra = Extras.ParseExtra(extraString);
                if (extra.Type != Extras.Types.Invalid)
                {
                    potentialExtras.Add(extra.Id, extra);
                }
            }
            try
            {
                return potentialExtras[id];
            } catch (Exception) {
                return new Extra(Type: Extras.Types.Invalid, Text: "No such extra with Id of" + id);
            }
            }
    }
}

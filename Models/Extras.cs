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
            Radio,
            Addon,
            Empty,
            Appendix
        }
        //todo ensure that customers cannot buy hidden products
        public record Extra(
            Types Type = Types.Invalid,
            string Text = "",
            string Id = "",
            double Cost = 0,
            string HintText = "",
            Dictionary<string, double>? RadioOptions = null
            );

        public static Extra ParseExtra(string extraString)
        {
            void validateExtraSections(string[] parts, int length)
            {
                if (parts.Length != length) throw new Exception(parts.Length + " is not a valid number of arguments for " + parts[0]);
            }
            try
            {
                var parts = extraString.Split("::");

                switch (parts[0].ToLower())
                {
                    case "per letter"://per letter::<id>::<display text>::<cost per letter>
                        validateExtraSections(parts, 4);
                        return new Extra(Type: Types.PerLetter, Id: parts[1], Text:parts[2], Cost: Double.Parse(parts[3]));
                    case "text field"://text field::<id>::<display text>::<hint text>
                        validateExtraSections(parts, 4);
                        return new Extra(Type: Types.TextField, Id: parts[1], Text: parts[2], HintText: parts[3]);
                    case "radio"://radio::<id>::<display text>::<repeat <radio title>/:/<radio cost> as needed, separating with /:/>
                        validateExtraSections(parts, 4);
                        var radioParts = parts[3].Split("/:/");
                        if (radioParts.Length % 2 != 0) throw new Exception("Odd number of radio parts");
                        //todo make addons show up on the checkout page
                        Dictionary<string, double> radioOptions = new();
                        for(var i = 0; i < radioParts.Length / 2; i++)
                        {
                            radioOptions.Add(radioParts[i * 2], double.Parse(radioParts[i * 2 + 1]));
                        }

                        if (radioOptions.Count == 0) throw new Exception("At least one radio option required, zero found");
                        
                        return new Extra(Type: Types.Radio, Id: parts[1], Text: parts[2], RadioOptions: radioOptions);
                    case "addon"://addon::<product id to list as addon>
                        validateExtraSections(parts, 2);
                        return new Extra(Type: Types.Addon, Id: parts[1]);
                    case "appendix"://addon::<product id to append>
                        validateExtraSections(parts, 2);
                        return new Extra(Type: Types.Appendix, Id: parts[1]);
                    case "":
                        return new Extra(Type: Types.Empty);
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

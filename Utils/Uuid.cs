namespace SuopCommerce.Utils
{
    public static class Uuid
    {
        public static string GetUuid()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}

namespace SuopCommerce.Utils.Data
{
    //this caching util assumes that the program it is running on is the only source of changes to the database. Updated db values must be set with set.
    public static class CachingUtil
    {
        private static Dictionary<string, object> Cache = new();
        private static object? Get(string id)
        {
            try {
                return Cache[id];
            } catch {
                return null;
            }
        }
        
        private static void Set(string id, object value)
        {
            try
            {
                Cache[id] = value;
            }
            catch
            {
                Cache.Add(id, value);
            }
        }
    }
}

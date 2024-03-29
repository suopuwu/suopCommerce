﻿using suopCommerce.Models;

namespace SuopCommerce.Utils.Data
{
    public static class BulkImageRetriever
    {
        public static async Task<Image?[]> FromListAsync(int[]? idList, bool pruned = true)
        {//performance: it may be necessary to find a way to prune all items simultaneously.
            if(idList == null || idList.Length == 0)
            {
                return Array.Empty<Image?>();
            }//todo add pagination for products and such to avoid overloading the database
            using var db = new StoreContext();
            List<Image?> returnList = new();
            foreach(int id in idList)
            {
                var tempImage = await db.Images.FindAsync(id);
                if(tempImage != null)
                {
                    returnList.Add(tempImage); 
                } else if (!pruned) {
                    returnList.Add(null);
                }
            }
            return returnList.ToArray();
            
        }
    }
}

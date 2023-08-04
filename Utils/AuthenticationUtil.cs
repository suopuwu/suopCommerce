using Newtonsoft.Json;
using suopCommerce.Models;
using System.Text.Json;

namespace SuopCommerce.Utils
{
    public static class AuthenticationUtil
    {
        private static string? CachedPassword = null;

        //tries to get the password, adding a default one to the db if it doesn't exist yet.
        private static AdminPassword TryGetPassword()
        {
            StoreContext db = new();
            var returnPassword = db.AdminPasswords.Find(1);
            if (returnPassword == null)
            {
                var freshPassword = new AdminPassword();
                db.AdminPasswords.Add(freshPassword);
                db.SaveChanges();
                returnPassword = db.AdminPasswords.Find(1);
            }
            return returnPassword!;
            
        }

        public static bool IsAdmin(string? password)
        {
            if (password == null)
            {
                return false;
            }

            CachedPassword = TryGetPassword().Value;
            return CachedPassword == password;
        }

        public static string SetPassword(string newPassword)
        {
            StoreContext db = new();
            TryGetPassword();
            db.AdminPasswords.Find(1)!.Value = newPassword;
            db.SaveChanges();
            return JsonConvert.SerializeObject(new { success = true, message = "Password changed"});
        }

        public static string? AuthenticateApiCall(HttpContext context)
        {
            if (!IsAdmin(context.Request.Cookies["admin-password"]))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return JsonConvert.SerializeObject(new { success = false, message = "Invalid old password" });
            } else
            {
                return null;
            }
        }
    }

}

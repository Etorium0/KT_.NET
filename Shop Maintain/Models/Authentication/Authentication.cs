using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Shop_Maintain.Models.Authentication
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Lấy thông tin từ session
            var username = context.HttpContext.Session.GetString("UserName");
            var userType = context.HttpContext.Session.GetInt32("UserType");
            
            // Lấy đường dẫn hiện tại
            var path = context.HttpContext.Request.Path.ToString().ToLower();
            
            // Debug log
            System.Diagnostics.Debug.WriteLine($"Current Path: {path}");
            System.Diagnostics.Debug.WriteLine($"UserName: {username}");
            System.Diagnostics.Debug.WriteLine($"UserType: {userType}");

            // Kiểm tra nếu path chứa "admin"
            if (path.Contains("/admin"))
            {
                // Nếu chưa đăng nhập hoặc không phải admin
                if (string.IsNullOrEmpty(username) || userType != 1)
                {
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"Controller", "Access" },
                            {"Action", "Login" }
                        });
                    return;
                }
            }
            
            // Cho các trường hợp khác
            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Controller", "Access" },
                        {"Action", "Login" }
                    });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

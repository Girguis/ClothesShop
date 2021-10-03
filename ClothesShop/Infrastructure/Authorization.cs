using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Helpers;

namespace ClothesShop.Infrastructure
{
    public class Authorization : AuthorizeAttribute
    {
        JobRolesRepo _JobTypesRepo = new JobRolesRepo(); // my entity  
        private readonly string _RoleName;
        private readonly RoleType _RoleType;
        public Authorization(string roleName, RoleType roleType)
        {
            _RoleName = roleName;
            _RoleType = roleType;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return RolesHelper.CheckRoleRight(_RoleName, _RoleType);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" },
                   { "area", "" }
               });
        }
    }
}
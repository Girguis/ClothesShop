using System.Collections.Generic;
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
        private readonly string[] _RoleName;
        private readonly RoleType _RoleType;
        public Authorization(string roleNames, RoleType roleType)
        {
            _RoleName = roleNames.Split(new string[] {","},System.StringSplitOptions.None);
            _RoleType = roleType;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var res = false;
            foreach(var roleName in _RoleName)
            {
                res |= RolesHelper.CheckRoleRight(roleName, _RoleType);
            }
            return res;
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
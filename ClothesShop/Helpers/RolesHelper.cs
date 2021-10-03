using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BLL.Repositories;
using ClothesShop.Enums;
using DAL;

namespace ClothesShop.Helpers
{
    public static class RolesHelper
    {
        public static bool CheckRoleRight(string roleName, RoleType roleType)
        {
            try
            {
                if (HttpContext.Current.Session["UserID"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["UserID"].ToString()))
                    return false;
                var userId = long.Parse(HttpContext.Current.Session["UserID"].ToString());
                EmployeesRepo repo = new EmployeesRepo();
                Employee employee = repo.GetByID(userId);
                if (employee == null)
                    return false;
                var jobTypeId = employee.JobTypeID;

                IEnumerable<JobRole> jobRoles = new List<JobRole>();
                if (HttpContext.Current.Session["Roles"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["Roles"].ToString()))
                {
                    JobRolesRepo _JobTypesRepo = new JobRolesRepo();
                    jobRoles = _JobTypesRepo.GetByJobID(jobTypeId);
                    HttpContext.Current.Session["Roles"] = jobRoles;
                }
                else
                {
                    jobRoles = (List<JobRole>)HttpContext.Current.Session["Roles"];
                }

                if (jobRoles == null || jobRoles.Count() <= 0)
                    return false;
                var jobRole = jobRoles.Where(j => j.Role.Name == roleName).FirstOrDefault();
                if (jobRole == null)
                    return false;
                if (roleType == RoleType.Add)
                    return jobRole.Add == true;
                if (roleType == RoleType.Edit)
                    return jobRole.Edit == true;
                if (roleType == RoleType.Delete)
                    return jobRole.Delete == true;
                if (roleType == RoleType.View)
                    return jobRole.View == true;
                if (roleType == RoleType.Details)
                    return jobRole.View == true;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return false;
        }

        public static bool CheckRoleRight(string roleName)
        {
            try
            {
                if (HttpContext.Current.Session["UserID"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["UserID"].ToString()))
                    return false;
                var userId = long.Parse(HttpContext.Current.Session["UserID"].ToString());
                EmployeesRepo repo = new EmployeesRepo();
                Employee employee = repo.GetByID(userId);
                if (employee == null)
                    return false;
                var jobTypeId = employee.JobTypeID;

                IEnumerable<JobRole> jobRoles = new List<JobRole>();
                if (HttpContext.Current.Session["Roles"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["Roles"].ToString()))
                {
                    JobRolesRepo _JobTypesRepo = new JobRolesRepo();
                    jobRoles = _JobTypesRepo.GetByJobID(jobTypeId);
                    HttpContext.Current.Session["Roles"] = jobRoles;
                }
                else
                {
                    jobRoles = (List<JobRole>)HttpContext.Current.Session["Roles"];
                }

                if (jobRoles == null || jobRoles.Count() <= 0)
                    return false;
                var jobRole = jobRoles.Where(j => j.Role.Name == roleName).FirstOrDefault();
                if (jobRole == null)
                    return false;
                return jobRole.Add == true || jobRole.Edit == true || jobRole.Delete == true || jobRole.View == true || jobRole.Details == true;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return false;
        }
    }
}
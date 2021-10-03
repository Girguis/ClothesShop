using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Areas.Settings.Models;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;
using DAL;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Areas.Settings.Controllers
{
    [Authentication]
    public class RolesController : Controller
    {
        private readonly JobRolesRepo _JobRolesRepo;
        public RolesController()
        {
            _JobRolesRepo = new JobRolesRepo();
        }
        // GET: Settings/Roles/Edit/5

        [Authorization("Roles", (RoleType.Edit))]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IEnumerable<JobRole> roles = _JobRolesRepo.GetByJobID(id.Value);
            if (roles == null)
            {
                return HttpNotFound();
            }
            var rolesViewModel = roles.Select(r => GetJobRoleViewModel(r));
            return View(rolesViewModel.ToList());
        }

        // POST: Settings/Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Roles", (RoleType.Edit))]
        public ActionResult Edit(List<JobRoleViewModel> roles)
        {
            if (ModelState.IsValid)
            {
                var jobRoles = roles.Select(r => GetJobRoleModel(r));
                _JobRolesRepo.Update(jobRoles.ToList());
                return RedirectToAction("Index", "Setting");
            }
            return View(roles);
        }
        private JobRoleViewModel GetJobRoleViewModel(JobRole r)
        {
            return new JobRoleViewModel()
            {
                Add = r.Add.HasValue ? r.Add.Value : false,
                Edit = r.Edit.HasValue ? r.Edit.Value : false,
                Delete = r.Delete.HasValue ? r.Delete.Value : false,
                View = r.View.HasValue ? r.View.Value : false,
                Details = r.Details.HasValue ? r.Details.Value : false,
                JobTypeID = r.JobTypeID.HasValue ? r.JobTypeID.Value : 0,
                ID = r.ID,
                Name = r.Role != null ? r.Role.ArabicName : "",
                RoleID = r.RoleID.HasValue ? r.RoleID.Value : 0,
            };
        }
        private JobRole GetJobRoleModel(JobRoleViewModel r)
        {
            return new JobRole()
            {
                Add = r.Add,
                Edit = r.Edit,
                Delete = r.Delete,
                View = r.View,
                Details = r.Details,
                JobTypeID = r.JobTypeID,
                ID = r.ID,
                RoleID = r.RoleID,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL.Repositories
{
    public class JobRolesRepo
    {
        private readonly ClothesShopEntities ClothesShopEntities;
        public JobRolesRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public IEnumerable<JobRole> GetAll()
        {
            return ClothesShopEntities.JobRoles;
        }
        public IEnumerable<JobRole> GetByJobID(int jobTypeId)
        {
            return ClothesShopEntities.JobRoles.Where(j => j.JobTypeID == jobTypeId);
        }

        public JobRole GetByID(long id)
        {
            return ClothesShopEntities.JobRoles.Find(id);
        }

        public bool Update(List<JobRole> roles)
        {
            try
            {
                foreach(var role in roles)
                {
                    ClothesShopEntities.Entry(role).State = System.Data.Entity.EntityState.Modified;
                }
                
                int result = ClothesShopEntities.SaveChanges();
                return (result > 0);
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
    }
}

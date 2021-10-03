using System;
using System.Collections.Generic;
using DAL;

namespace BLL.Repositories
{
    public class CitiesRepo : IRepository<City>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public CitiesRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public IEnumerable<City> GetAll()
        {
            return ClothesShopEntities.Cities;
        }

        public City GetByID(long id)
        {
            throw new NotImplementedException();
        }

        public City Add(City obj)
        {
            throw new NotImplementedException();
        }

        public bool Update(City obj)
        {
            throw new NotImplementedException();
        }

        public bool Delete(long id)
        {
            throw new NotImplementedException();
        }
    }
}

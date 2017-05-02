using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DTO;

namespace WithoutPath.DAL
{
    public partial class SqlRepository
    {
        public IQueryable<ShipType> ShipTypes
        {
            get
            {
                return Db.ShipTypes;
            }
        }

        public IResult CreateShipType(ShipType instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.ShipTypes.Add(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = "id not equal 0"
                };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        public IResult UpdateShipType(ShipType instance)
        {
            try
            {
                var cache = Db.ShipTypes.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.EveID = instance.EveID;
                    cache.Name = instance.Name;

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("ShipType with ID {0} not found", instance.Id)
                };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        public IResult RemoveShipType(int idShipType)
        {
            try
            {
                var instance = Db.ShipTypes.FirstOrDefault(p => p.Id == idShipType);
                if (instance != null)
                {
                    Db.ShipTypes.Remove(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("ShipType with ID {0} not found", idShipType)
                };

            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }
    }
}
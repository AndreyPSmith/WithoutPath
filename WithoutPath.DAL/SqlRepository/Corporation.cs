using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DTO;

namespace WithoutPath.DAL
{
    public partial class SqlRepository
    {
        public IQueryable<Corporation> Corporations
        {
            get
            {
                return Db.Corporations;
            }
        }

        public IResult CreateCorporation(Corporation instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.Corporations.Add(instance);
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

        public IResult UpdateCorporation(Corporation instance)
        {
            try
            {
                var cache = Db.Corporations.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.EveID = instance.EveID;
                    cache.Name = instance.Name;
                    cache.Ticker = instance.Ticker;

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Corporation with ID {0} not found", instance.Id)
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

        public IResult RemoveCorporation(int idCorporation)
        {
            try
            {
                var instance = Db.Corporations.FirstOrDefault(p => p.Id == idCorporation);
                if (instance != null)
                {
                    Db.Characters.Where(x => x.CorporationID == idCorporation).ToList().ForEach(x => {
                        x.IsDeleted = true;
                    });

                    Db.Corporations.Remove(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Corporation with ID {0} not found", idCorporation)
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

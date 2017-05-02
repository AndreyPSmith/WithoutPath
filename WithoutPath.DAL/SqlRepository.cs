using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WithoutPath.DAL
{
    public partial class SqlRepository : IRepository
    {
        public WithoutPathEntities Db { get; set; }

        public SqlRepository()
        {
            Db = new WithoutPathEntities();
        }

        public void UdateDBContext()
        {
            Db = new WithoutPathEntities();
        }

        public void SaveChanges()
        {
            Db.SaveChanges();
        }
   
    }
}

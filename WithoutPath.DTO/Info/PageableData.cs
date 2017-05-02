using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutPath.DTO
{
    public class PageableData<T> where T : class
    {
        protected static int ItemPerPageDefault = 20;

        public IEnumerable<T> List { get; set; }

        public int PageNo { get; set; }

        public int CountPage { get; set; }

        public int ItemPerPage { get; set; }

        public PageableData(IQueryable<T> queryableSet, int page, int itemPerPage = 0)
        {
            if (itemPerPage == 0)
            {
                itemPerPage = ItemPerPageDefault;
            }
            ItemPerPage = itemPerPage;

            PageNo = page;
            var count = queryableSet.Count();

            CountPage = (int)decimal.Remainder(count, itemPerPage) == 0 ? count / itemPerPage : count / itemPerPage + 1;
            List = queryableSet.Skip((PageNo - 1) * itemPerPage).Take(itemPerPage);
        }
    }
}

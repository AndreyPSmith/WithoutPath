using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutPath.DTO
{
    public class SimpleResult : IResult
    {
        public bool IsError { get; set; }

        public string Message { get; set; }
    }
}

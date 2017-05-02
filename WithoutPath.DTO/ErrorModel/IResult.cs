using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutPath.DTO
{
    public interface IResult
    {
        bool IsError { get; set; }

        string Message { get; set; }
    }
}

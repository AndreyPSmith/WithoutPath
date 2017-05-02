using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutPath.DTO
{
    public class StatusFlags
    {
        [Flags]
        public enum Flags
        {
            /// <summary>
            /// Все хорошо
            /// </summary>
            None = 0,

            /// <summary>
            /// Половина массы
            /// </summary>
            HalfMass = 1,

            /// <summary>
            /// Крит по массе
            /// </summary>
            CriticalMass = 2,

            /// <summary>
            /// Крит по времени
            /// </summary>
            CriticalTime = 4
        }
    }

    public class LinksModel
    { 
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Time { get; set; }
        public int Status { get; set; }

    }
}

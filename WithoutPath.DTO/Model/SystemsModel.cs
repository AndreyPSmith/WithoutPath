using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutPath.DTO
{
    public class SystemsModel
    {
        public int Id { get; set; }
        public bool IsWormhole { get; set; }
        public long? EveID { get; set; }
        public string Name { get; set; }
        public double? Security { get; set; }
        public string Note { get; set; }
        public bool Warning { get; set; }
        public bool IsHome { get; set; }
        public List<CharacterModel> Characters { get; set; }
        public SystemTypeModel SystemType { get; set; }

        public List<StaticModel> Statics { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutPath.DTO
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int CharacterID { get; set; }
        public int PostID { get; set; }
        public string Content { get; set; }
        public System.DateTime AddedDate { get; set; }
        public  CharacterModel Character { get; set; }
        public PostModel Post { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithoutPath.DAL
{
    public class UserProxy
    {
        public UserProxy()
        {
            this.Characters = new List<Character>();
            this.UserRoles = new List<UserRole>();
        }

        [Key]
        public Nullable<int> Id { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string Password { get; set; }
        public string ActivatedLink { get; set; }
        public Nullable<System.DateTime> ActivatedDate { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<bool> Banned { get; set; }

        public ICollection<Character> Characters { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}

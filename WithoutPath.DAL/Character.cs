//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WithoutPath.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Character
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Character()
        {
            this.Comments = new HashSet<Comment>();
            this.Links = new HashSet<Link>();
            this.Posts = new HashSet<Post>();
        }
    
        public int Id { get; set; }
        public Nullable<int> CorporationID { get; set; }
        public string Name { get; set; }
        public Nullable<int> SystemID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Ship { get; set; }
        public long EveID { get; set; }
        public Nullable<bool> IsMain { get; set; }
        public Nullable<int> TokenID { get; set; }
        public Nullable<int> ShipTypeID { get; set; }
        public Nullable<bool> IsOnline { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual Corporation Corporation { get; set; }
        public virtual ShipType ShipType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Link> Links { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }
        public virtual SpaceSystem SpaceSystem { get; set; }
        public virtual User User { get; set; }
    }
}

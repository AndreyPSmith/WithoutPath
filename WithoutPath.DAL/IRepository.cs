using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DTO;

namespace WithoutPath.DAL
{
    public interface IRepository 
    {
        void SaveChanges();

        void UdateDBContext();

        #region Role

        IQueryable<Role> Roles { get; }

        IResult CreateRole(Role instance);

        IResult RemoveRole(int idRole);

        #endregion

        #region UserRole

        IQueryable<UserRole> UserRoles { get; }

        IResult CreateUserRole(UserRole instance);

        IResult RemoveUserRole(int idUserRole);

        #endregion

        #region User

        IQueryable<User> Users { get; }

        IResult CreateUser(User instance);

        User GetUser(string email);

        User GetUser(int idUser);

        User Login(string email, string password);

        IResult UpdateUser(User instance);

        IResult ActivateUser(User instance);

        IResult RemoveUser(int idUser);

        IResult ChangePassword(User instance);

        #endregion

        #region SpaceSystem

        IQueryable<SpaceSystem> SpaceSystems { get; }

        IResult CreateSpaceSystem(SpaceSystem instance);

        IResult UpdateSpaceSystem(SpaceSystem instance);

        IResult RemoveSpaceSystem(int idSpaceSystem);

        IResult RemoveSpaceSystemLinks(int id);

        #endregion

        #region Corporation

        IQueryable<Corporation> Corporations { get; }

        IResult CreateCorporation(Corporation instance);

        IResult UpdateCorporation(Corporation instance);

        IResult RemoveCorporation(int idCorporation);

        #endregion 

        #region Link

        IQueryable<Link> Links { get; }

        IResult CreateLink(Link instance);

        IResult UpdateLink(Link instance);

        IResult RemoveLink(int idLink);

        IResult RemoveOldLinks();

        #endregion

        #region Character

        IQueryable<Character> Characters { get; }

        IResult CreateCharacter(Character instance, bool IsSave = true);

        IResult UpdateCharacter(Character instance, bool IsSave = true);

        IResult RemoveCharacter(int idCharacter, bool IsSave = true);

        IResult SetMainCharacter(Character instance);

        #endregion

        #region SystemType

        IQueryable<SystemType> SystemTypes { get; }

        IResult CreateSystemType(SystemType instance);

        IResult RemoveSystemType(int idSystemType);

        #endregion

        #region SpaceSystemStatic

        IQueryable<SpaceSystemStatic> SpaceSystemStatics { get; }

        IResult CreateSpaceSystemStatic(SpaceSystemStatic instance);

        IResult RemoveSpaceSystemStatic(int idSpaceSystemStatic);

        #endregion

        #region Static

        IQueryable<Static> Statics { get; }

        IResult CreateStatic(Static instance);

        IResult RemoveStatic(int idStatic);

        #endregion

        #region Token

        IQueryable<Token> Tokens { get; }

        IResult CreateToken(Token instance, bool IsSave = true);

        IResult UpdateToken(Token instance, bool IsSave = true);

        IResult RemoveToken(int idToken, bool IsSave = true);

        #endregion

        #region Token

        IQueryable<ShipType> ShipTypes { get; }

        IResult CreateShipType(ShipType instance);

        IResult UpdateShipType(ShipType instance);

        IResult RemoveShipType(int idShipType);

        #endregion
        #region Post

        IQueryable<Post> Posts { get; }

        IResult CreatePost(Post instance);

        IResult UpdatePost(Post instance);

        IResult RemovePost(int idPost);

        IResult VerifyPost(int Id, bool IsVerified);

        #endregion

        #region Comment

        IQueryable<Comment> Comments { get; }

        IResult CreateComment(Comment instance);

        IResult UpdateComment(Comment instance);

        IResult RemoveComment(int idComment);

        #endregion

    }
}

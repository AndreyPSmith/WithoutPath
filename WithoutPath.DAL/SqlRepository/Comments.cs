using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DTO;

namespace WithoutPath.DAL
{
    public partial class SqlRepository
    {
        public IQueryable<Comment> Comments
        {
            get
            {
                return Db.Comments;
            }
        }

        public IResult CreateComment(Comment instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    instance.AddedDate = DateTime.Now;
                    Db.Comments.Add(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = "id not equal 0"
                };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        public IResult UpdateComment(Comment instance)
        {
            try
            {
                var cache = Db.Comments.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {

                    cache.CharacterID = instance.CharacterID;
                    cache.PostID = instance.PostID;
                    cache.Content = instance.Content;
                    cache.AddedDate = DateTime.Now;

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Comment with ID {0} not found", instance.Id)
                };
            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }

        public IResult RemoveComment(int idComment)
        {
            try
            {
                var instance = Db.Comments.FirstOrDefault(p => p.Id == idComment);
                if (instance != null)
                {
                    Db.Comments.Remove(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Comment with ID {0} not found", idComment)
                };

            }
            catch (Exception e)
            {
                return new SimpleResult
                {
                    IsError = true,
                    Message = e.Message
                };
            }
        }
    }
}
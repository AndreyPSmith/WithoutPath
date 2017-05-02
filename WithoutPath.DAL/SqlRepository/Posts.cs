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
        public IQueryable<Post> Posts
        {
            get
            {
                return Db.Posts;
            }
        }

        public IResult CreatePost(Post instance)
        {
            try
            {
                if (instance.Id == 0)
                {
                    Db.Posts.Add(instance);
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

        public IResult UpdatePost(Post instance)
        {
            try
            {
                var cache = Db.Posts.FirstOrDefault(p => p.Id == instance.Id);
                if (cache != null)
                {
                    cache.Header = instance.Header;
                    cache.Content = instance.Content;
                    cache.Height = instance.Height;
                    cache.Width = instance.Width;
                    cache.IsFixed = instance.IsFixed;
                    cache.IsInternal = instance.IsInternal;
                    cache.Picture = instance.Picture;
                    cache.IsVerified = instance.IsVerified;

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Post with ID {0} not found", instance.Id)
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

        public IResult VerifyPost(int Id, bool IsVerified)
        {
            try
            {
                var cache = Db.Posts.FirstOrDefault(p => p.Id == Id);
                if (cache != null)
                {
                    cache.IsVerified = IsVerified;

                    Db.SaveChanges();
                    return new SimpleResult { IsError = false };
                }
                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Post with ID {0} not found", Id)
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

        public IResult RemovePost(int idPost)
        {
            try
            {
                var instance = Db.Posts.FirstOrDefault(p => p.Id == idPost);
                if (instance != null)
                {
                    Db.Posts.Remove(instance);
                    Db.SaveChanges();

                    return new SimpleResult { IsError = false };
                }

                return new SimpleResult
                {
                    IsError = true,
                    Message = string.Format("Post with ID {0} not found", idPost)
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
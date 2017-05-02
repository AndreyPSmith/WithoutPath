using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WithoutPath.DAL;
using WithoutPath.DTO;

namespace WithoutPath.Mappers
{
    public class CommonMapper : IMapper
    {
        AutoMapper.IMapper mapper { get; set; }

        public CommonMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserProxy>();
                cfg.CreateMap<UserProxy, User>();
                cfg.CreateMap<Post, PostModel>();
                cfg.CreateMap<PostModel, Post>();
                cfg.CreateMap<Character, CharacterModel>();
                cfg.CreateMap<CharacterModel, Character>();
                cfg.CreateMap<CommentModel, Comment>();
                cfg.CreateMap<Comment, CommentModel>();
            });

            mapper = config.CreateMapper();
        }

        public object Map(object source, Type sourceType, Type destinationType)
        {
            return mapper.Map(source, sourceType, destinationType);
        }
    }
}
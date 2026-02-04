using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Dto;
using Domain.Models.Entities;

namespace Infrastructure.Mapper
{
    public static class PostCreatePersistenceMapper
    {
        public static PostCreatePersistenceDto ToPersistenceDto(this Post post)
        {
            return new PostCreatePersistenceDto(
                post.Title,
                post.Content
                );
        }

        public static Post ToEntity(this PostCreatePersistenceDto dto)
        {
            return new Post(
                dto.Title,
                dto.Content
                );
        }
    }
}
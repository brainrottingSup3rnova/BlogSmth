using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Dto;
using Domain.Models.Entities;

namespace Infrastructure.Mapper
{
    public static class PostReadPersistenceMapper
    {
        public static PostReadPersistenceDto ToPersistenceDto(this Post post)
        {
            return new PostReadPersistenceDto(
                post.Id.ToString(),
                post.Title,
                post.Content,
                post.CreatedAt
                );
        }

        public static Post ToEntity(this PostReadPersistenceDto dto)
        {
            return new Post(
                new Guid(dto.Id),
                dto.Title,
                dto.Content,
                dto.CreatedAt
                );
        }
    }
}

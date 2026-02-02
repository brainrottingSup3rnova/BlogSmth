using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Models.Entities;


namespace Application.Mappers
{
    public static class PostReadMapper
    {
        public static PostReadDto ToReadDto(this Post post)
        {
            if(post != null)
            {
                return new PostReadDto(
                    post.Id,
                    post.Title,
                    post.Content,
                    post.CreatedAt
                    );
            }
            else
            {
                throw new ArgumentNullException(nameof(post), "Post cannot be null.");
            }
        }

        public static Post ToReadEntity(this PostReadDto postDto)
        {
            if(postDto != null)
            {
                return new Post(
                    postDto.Id,
                    postDto.Title,
                    postDto.Content,
                    postDto.CreatedAt
                    );
            }
            else
            {
                throw new ArgumentNullException(nameof(postDto), "PostDto cannot be null.");
            }
        }
    }
}

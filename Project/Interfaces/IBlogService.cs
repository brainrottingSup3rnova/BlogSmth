using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    //descrive tutti i metodi che costituiranno il service, utile per la build 
    public interface IBlogService
    {
        public Task CreatePostAsync(PostCreateDto postDto);
        public Task<PostCreateDto?> GetPostByIdAsync(string id);
        public Task<IEnumerable<PostReadDto>> GetAllPostsAsync();
        public Task UpdatePostAsync(string id, PostCreateDto postDto);
        public Task DeletePostAsync(string id);
    }
}

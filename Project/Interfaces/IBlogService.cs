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
        public Task CreateArticleAsync(PostCreateDto articleDto);

        public Task<PostReadDto?> GetArticleByIdAsync(string id);

        public Task<IEnumerable<PostReadDto>> GetAllArticlesAsync();

        public Task UpdateArticleAsync(string id, PostCreateDto articleDto);

        public Task DeleteArticleAsync(string id);
    }
}

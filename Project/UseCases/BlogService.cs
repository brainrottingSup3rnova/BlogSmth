using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using Application.Mappers;

namespace Application.UseCases
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;

        //dependency injection
        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task CreatePostAsync(PostCreateDto postDto)
        {
            var entity = postDto.ToCreateEntity();

            await _blogRepository.SaveAsync(entity);
        }

        public async Task<PostCreateDto?> GetPostByIdAsync(string id)
        {
            var entity = await _blogRepository.GetByIdAsync(id);

            if(entity == null)
            {
                return null;
            }
            return entity.ToCreateDto();
        }

        public async Task<IEnumerable<PostReadDto>> GetAllPostsAsync()
        {
            var entities = await _blogRepository.GetAllAsync();

            //map each entity to dto
            return entities.Select(e => e.ToReadDto());
        }

        public async Task UpdatePostAsync(string id, PostCreateDto postDto)
        {
            var entity = await _blogRepository.GetByIdAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Post with id {id} not found.");
            }
            entity.Title = postDto.Title;
            entity.Content = postDto.Content;

            await _blogRepository.UpdateAsync(entity);
        }

        public async Task DeletePostAsync(string id)
        {
            var post = await _blogRepository.GetByIdAsync(id);

            if (post == null)
            {
                throw new KeyNotFoundException($"Post with id {id} not found.");
            }

            await _blogRepository.DeleteAsync(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using Application.Interfaces;
using Domain.Models.Entities;

namespace Infrastructure.Repositories
{
    public class JsonBlogRepository : IBlogRepository
    {
        private readonly string _filePath = "blogposts.json";
        private Dictionary<string, Post> _postList = new Dictionary<string, Post>();
        private bool _isDataLoaded = false;

        private void EnsureDataLoaded()
        {
            if (_isDataLoaded)
            {
                return;
            }

            if (!File.Exists(_filePath))
            {
                _isDataLoaded = true;
                return;
            }

            var jsonData = File.ReadAllText(_filePath);
            var dtoList = System.Text.Json.JsonSerializer.Deserialize<List<PostReadPersistenceDto>>(jsonData);

            if (dtoList != null)
            {
                _postList = dtoList.ToDictionary(dto => dto.Id, dto => dto.ToEntity());
            }

            _isDataLoaded = true;
        }

        public async Task SaveAsync(Post post)
        {
            EnsureDataLoaded();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            EnsureDataLoaded();

            Post[] posts = new Post[_postList.Count];
            for(int i = 0; i < _postList.Count; i++)
            {
                posts[i] = _postList.ElementAt(i).Value;
            }

            return posts;
        }

        public async Task<Post?> GetByIdAsync(string id)
        {
            EnsureDataLoaded();

            Post? post;
            _postList.TryGetValue(id, out post);

            return post;
        }

        public async Task UpdateAsync(Post post)
        {
            EnsureDataLoaded();

            _postList[post.Id.ToString()] = post;
        }

        public async Task DeleteAsync(string id)
        {
            EnsureDataLoaded();

            _postList.Remove(id);
        }
    }
}

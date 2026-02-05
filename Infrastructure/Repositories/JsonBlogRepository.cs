using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using Application.Interfaces;
using Domain.Models.Entities;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class JsonBlogRepository : IBlogRepository
    {
        private readonly string _filePath = "blogposts.json";
        private Dictionary<string, Post> _postList = new Dictionary<string, Post>();
        private bool _isDataLoaded = false;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public JsonBlogRepository(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "blogposts.json");
        }

        //utile solo se usato soltanto in locale/da questa applicazione
        private async Task EnsureDataLoaded()
        {
            if (_isDataLoaded)
            {
                return;
            }

            if (!File.Exists(_filePath))
            {
                await using FileStream stream = File.OpenRead(_filePath);
                var postDto = await JsonSerializer.DeserializeAsync<Dictionary<string, PostPersistenceDto>>(stream);

                _postList = postDto!.Select(kv => kv.Value.ToEntity()).ToDictionary(post => post.Id.ToString(), post => post);
            }

            _isDataLoaded = true;
        }

        private async Task SaveDataAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                //TODO: FINISH THIS METHOD
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SaveAsync(Post post)
        {
            await EnsureDataLoaded();
            _postList.Add(post.Id.ToString(), post);
            await SaveDataAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            await EnsureDataLoaded();

            Post[] posts = new Post[_postList.Count];
            for(int i = 0; i < _postList.Count; i++)
            {
                posts[i] = _postList.ElementAt(i).Value;
            }

            return posts;
        }

        public async Task<Post?> GetByIdAsync(string id)
        {
            await EnsureDataLoaded();

            Post? post;

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            }
            else if (!_postList.ContainsKey(id))
            {
                throw new KeyNotFoundException($"Post with Id {id} not found.");
            }
            else
            {
                _postList.TryGetValue(id, out post);
                if (post == null)
                {
                    throw new ArgumentException(nameof(post));
                }
            }

            return post;
        }

        public async Task UpdateAsync(Post post)
        {
            await EnsureDataLoaded();

            if(post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            else
            {
                _postList[post.Id.ToString()] = post;
            }

            await SaveDataAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await EnsureDataLoaded();

            if(String.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            }
            else if(!_postList.ContainsKey(id))
            {
                throw new KeyNotFoundException($"Post with Id {id} not found.");
            }
            else
            {
                _postList.Remove(id);
            }

            await SaveDataAsync();
        }
    }
}

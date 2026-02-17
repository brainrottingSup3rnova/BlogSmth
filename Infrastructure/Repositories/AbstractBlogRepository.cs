using Domain.Models.Entities;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public abstract class AbstractBlogRepository
    {
        protected string _filePath;
        // Usiamo SemaphoreSlim invece di lock() perché lock non supporta l'await all'interno
        //Il SemaphoreSlim garantisce che l'operazione "Leggi -> Modifica -> Salva" sia atomica.
        //lock: Blocca il thread finché non ha finito.
        //SemaphoreSlim: "Sospende" il task senza bloccare il thread sottostante, permettendo al server di fare altro mentre aspetta il suo turno per scrivere sul file. È molto più scalabile.
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public AbstractBlogRepository(string? filePath = null) { }

        public abstract Task<Dictionary<string, PostPersistenceDto>> LoadFromFileAsync();
        public abstract Task SaveToFileAsync(Dictionary<string, PostPersistenceDto> articles);

        public async Task SaveAsync(Post article)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var dto = article.ToPersistenceDto();
                articles[article.Id.ToString()] = dto;
                await SaveToFileAsync(articles);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Post?> GetByIdAsync(string id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();

                if (!articles.TryGetValue(id, out var dto))
                    return null;

                return dto.ToEntity();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();

                return articles.Values
                    .Select(a => a.ToEntity())
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task UpdateAsync(Post article)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var idKey = article.Id.ToString();

                if (!articles.ContainsKey(idKey))
                    throw new InvalidOperationException($"Articolo con ID {article.Id} non trovato");

                articles[idKey] = article.ToPersistenceDto();

                await SaveToFileAsync(articles);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();

                if (!articles.ContainsKey(id))
                    throw new InvalidOperationException($"Articolo con ID {id} non trovato");

                articles.Remove(id);

                await SaveToFileAsync(articles);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Post?> GetPostByTitle(string title)
        {
            var allPosts = await GetAllAsync();

            var post = allPosts
                .FirstOrDefault(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            if (post == null)
            {
                throw new ArgumentNullException($"Nessun post trovato con il titolo che contiene '{title}'");
            }
            else
            {
                return post;
            }
        }

        public async Task<Post?> GetPostByContent(string content)
        {
            var allPosts = await GetAllAsync();

            var post = allPosts
                .FirstOrDefault(p => p.Content.Contains(content, StringComparison.OrdinalIgnoreCase));

            if (post == null)
            {
                throw new ArgumentNullException($"Nessun post trovato con il titolo che contiene '{content}'");
            }
            else
            {
                return post;
            }
        }

        public async Task<Post?> GetPostByDate(DateTime date)
        {
            var allPosts = await GetAllAsync();

            var post = allPosts
                .FirstOrDefault(p => p.CreatedAt.Date == date.Date);

            if (post == null)
            {
                throw new ArgumentNullException($"Nessun post trovato con il titolo che contiene '{date}'");
            }
            else
            {
                return post;
            }
        }

        public async Task<List<Post>> GetPostByPeriod(DateTime startDate, DateTime endDate)
        {
            var allPosts = await GetAllAsync();
            var post = allPosts
                .Where(p => p.CreatedAt.Date >= startDate.Date && p.CreatedAt.Date <= endDate.Date)
                .ToList();
            return post;
        }

        public async Task<int> CountPostsByDate(DateTime date)
        {
            var allPosts = await GetAllAsync();

            var postCount = allPosts
                .Where(p => p.CreatedAt.Date == date.Date)
                .Count();

            return postCount;
        }
    }
}

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
        private string _filePath;
        // Usiamo SemaphoreSlim invece di lock() perché lock non supporta l'await all'interno
        //Il SemaphoreSlim garantisce che l'operazione "Leggi -> Modifica -> Salva" sia atomica.
        //lock: Blocca il thread finché non ha finito.
        //SemaphoreSlim: "Sospende" il task senza bloccare il thread sottostante, permettendo al server di fare altro mentre aspetta il suo turno per scrivere sul file. È molto più scalabile.
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public JsonBlogRepository(string? filePath = null)
        {
            // Se non viene specificato un path, usa un file nella directory corrente
            _filePath = filePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlogProject",
                "articles.json"
            );

            // Crea la directory se non esiste
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Crea il file se non esiste
            if (!File.Exists(_filePath))
            {
                SaveToFileAsync(new Dictionary<string, PostPersistenceDto>());
            }
        }

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

        private async Task<Dictionary<string, PostPersistenceDto>> LoadFromFileAsync()
        {
            if (!File.Exists(_filePath))
                return new Dictionary<string, PostPersistenceDto>();

            using FileStream openStream = File.OpenRead(_filePath);

            // Se il file è vuoto, DeserializeAsync restituirebbe null o errore
            if (openStream.Length == 0)
                return new Dictionary<string, PostPersistenceDto>();

            return await JsonSerializer.DeserializeAsync<Dictionary<string, PostPersistenceDto>>(openStream, _jsonOptions)
                    ?? new Dictionary<string, PostPersistenceDto>();
        }

        private async Task SaveToFileAsync(Dictionary<string, PostPersistenceDto> articles)
        {
            using FileStream createStream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(createStream, articles, _jsonOptions);
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
        
    }
}

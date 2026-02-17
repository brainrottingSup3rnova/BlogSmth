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
    public class TxtBlogRepository : IBlogRepository
    {
        private string _filePath;
        // Usiamo SemaphoreSlim invece di lock() perché lock non supporta l'await all'interno
        //Il SemaphoreSlim garantisce che l'operazione "Leggi -> Modifica -> Salva" sia atomica.
        //lock: Blocca il thread finché non ha finito.
        //SemaphoreSlim: "Sospende" il task senza bloccare il thread sottostante, permettendo al server di fare altro mentre aspetta il suo turno per scrivere sul file. È molto più scalabile.
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private const string _separator = "|| SEPARATOR ||";
        private const string _newLine = "|| NEW LINE ||";

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public TxtBlogRepository(string? filePath = null)
        {
            // Se non viene specificato un path, usa un file nella directory corrente
            _filePath = filePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlogProject",
                "articles.txt"
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
                File.Create(_filePath).Dispose(); 
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
            await _semaphore.WaitAsync();
            try
            {
                Dictionary<string, PostPersistenceDto> articles = new Dictionary<string, PostPersistenceDto>();

                if (!File.Exists(_filePath))
                    return articles;

                string[] lines = await File.ReadAllLinesAsync(_filePath);

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] parts = line.Split(_separator, StringSplitOptions.None);

                    if (parts.Length != 4)
                        continue;

                    string id = parts[0];
                    string title = parts[1].Replace(_newLine, "\n");
                    string content = parts[2].Replace(_newLine, "\n");
                    long timeStamp = long.Parse(parts[3]);

                    PostPersistenceDto dto = new PostPersistenceDto(id, title, content, timeStamp);
                    articles[id] = dto;
                }

                return articles;
            }
            catch (Exception ex)
            {
                // Log dell'errore
                Console.Error.WriteLine($"Errore durante la lettura del file: {ex.Message}");
                return new Dictionary<string, PostPersistenceDto>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task SaveToFileAsync(Dictionary<string, PostPersistenceDto> articles)
        {
            List<string> lines = new List<string>();

            foreach (var kvp in articles)
            {
                PostPersistenceDto dto = kvp.Value;

                string title = dto.Title.Replace("\n", _newLine) ?? string.Empty;
                string content = dto.Content.Replace("\n", _newLine) ?? string.Empty;

                string line = $"{dto.Id}{_separator}{title}{_separator}{content}{_separator}{dto.TimeStamp}";

                lines.Add(line);
            }

            await File.WriteAllLinesAsync(_filePath, lines);
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

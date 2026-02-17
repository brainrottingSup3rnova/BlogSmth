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
    public class TxtBlogRepository : AbstractBlogRepository
    {
        private const string _separator = "|| SEPARATOR ||";
        private const string _newLine = "|| NEW LINE ||";

        public TxtBlogRepository(string? filePath = null) : base(filePath)
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

        public override async Task<Dictionary<string, PostPersistenceDto>> LoadFromFileAsync()
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

        public override async Task SaveToFileAsync(Dictionary<string, PostPersistenceDto> articles)
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
    }
}

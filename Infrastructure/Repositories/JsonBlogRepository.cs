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
    public class JsonBlogRepository : AbstractBlogRepository
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public JsonBlogRepository(string? filePath = null) :base (filePath)
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

        public override async Task<Dictionary<string, PostPersistenceDto>> LoadFromFileAsync()
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

        public override async Task SaveToFileAsync(Dictionary<string, PostPersistenceDto> articles)
        {
            using FileStream createStream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(createStream, articles, _jsonOptions);
        }
    }
}

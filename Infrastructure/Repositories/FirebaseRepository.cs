using Application.Interfaces;
using Domain.Models.Entities;
using Firebase.Database;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.WebSockets;

namespace Infrastructure.Repositories
{
    public class FirebaseRepository : IBlogRepository
    {
        //proprietà dellla nostra repo
        private readonly FirebaseClient _firebaseClient; //ci permette di connetterci al nostro database
        public const string ArticlesNode = "articles";

        //costruttore che inizializza il client firebase con l'URL del database
        public FirebaseRepository(string firebaseUrl)
        {
            //gli diciamo a quale database fa riferimento
            _firebaseClient = new FirebaseClient(firebaseUrl);
        }

        public async Task DeleteAsync(string id)
        {
            await _firebaseClient
                .Child(ArticlesNode)
                .Child(id)
                .DeleteAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            // Recupera tutti i nodi sotto "articles" e li mappa in BlogPostPersistenceDto
            var dtos = await _firebaseClient
                .Child(ArticlesNode) //gli dico il nodo che fa da root
                .OnceAsync<PostPersistenceDto>(); //ritorna tutti i figli di articlesNode

            // Mappa i DTO in entità Post, ordina per CreatedAt in ordine decrescente e restituisce la lista
            return dtos
                .Select(m => m.Object.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public async Task<Post?> GetByIdAsync(string id)
        {
            var dto = await _firebaseClient
                .Child(ArticlesNode)
                .Child(id)
                .OnceSingleAsync<PostPersistenceDto>();

            if (dto == null)
                return null;
            return dto.ToEntity();
        }

        public async Task SaveAsync(Post article)
        {
            await _firebaseClient
                .Child(ArticlesNode)
                .Child(article.Id.ToString())
                .PutAsync(article.ToPersistenceDto());
        }

        public async Task UpdateAsync(Post article)
        {
            await _firebaseClient
                .Child(ArticlesNode)
                .Child(article.Id.ToString())
                .PutAsync(article.ToPersistenceDto());
        }

        public async Task<Post?> GetPostByTitleAsync(string title)
        {
            var dtos = await GetAllAsync();
            return dtos.
                FirstOrDefault(p => p.Title.Equals(title));
        }

        public async Task<Post?> GetPostByContentAsync(string content)
        {
            var dtos = await GetAllAsync();
            return dtos.
                FirstOrDefault(p => p.Content.Equals(content));
        }

        public async Task<Post?> GetPostByCreatedAtAsync(long createdAt)
        {
            var dtos = await GetAllAsync();
            return dtos.
                FirstOrDefault(p => p.CreatedAt.Ticks == createdAt);
        }

        public async Task<int> CountByDateAsync(DateTime date)
        {
            var dtos = await GetAllAsync();
            return dtos.Count(p => p.CreatedAt.Date == date.Date);
        }

        public async Task<IEnumerable<Post>> GetInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var dtos = await GetAllAsync();
            return dtos.Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);
        }
    }
}

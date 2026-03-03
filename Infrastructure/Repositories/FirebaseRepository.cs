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

namespace Infrastructure.Repositories
{
    public class FirebaseRepository : IBlogRepository
    {
        //proprietà dellla nostra repo
        private readonly FirebaseClient _firebaseClient;
        public const string ArticlesNode = "articles";

        //costruttore che inizializza il client firebase con l'URL del database
        public FirebaseRepository(string firebaseUrl)
        {
            //gli diciamo a quale databse fa riferimento
            _firebaseClient = new FirebaseClient(firebaseUrl);
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            // Recupera tutti i nodi sotto "articles" e li mappa in BlogPostPersistenceDto
            var dtos = await _firebaseClient
                .Child(ArticlesNode)
                .OnceAsync<PostPersistenceDto>();

            // Mappa i DTO in entità Post, ordina per CreatedAt in ordine decrescente e restituisce la lista
            return dtos
                .Select(m => m.Object.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public Task<Post?> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(Post article)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Post article)
        {
            throw new NotImplementedException();
        }
    }
}

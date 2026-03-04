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

            if(dto == null)
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
    }
}

using Application.Dto;
using Application.Interfaces;   
using Application.UseCases;
using Domain.Models.Entities;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace BlogConsole
{
    internal class BlogProgram
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Blog Manager...");

            using IHost host = Host.CreateDefaultBuilder(args) //lo using garantisce che il host venga correttamente "smaltito" alla fine del blocco in automatico
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<IBlogRepository>(sp => //aggiunge un service, creandone un'istanza
                        new TxtBlogRepository());

                    services.AddScoped<IBlogService, BlogService>();
                })
                .Build();

            Console.WriteLine("Dependency Injection successfully configured!\n");
            Console.WriteLine("Press a button to start...");
            Console.ReadKey();
            Console.Clear();

            await RunApp(host.Services);
        }

        // ============================================================================
        // FUNZIONE PRINCIPALE DELL'APPLICAZIONE
        // ============================================================================
        static async Task RunApp(IServiceProvider services)
        {
            // Risolviamo il servizio dall'Application Layer 
            var blogService = services.GetRequiredService<IBlogService>();

            while (true)
            {
                ShowMenu();

                var choice = Console.ReadLine()?.Trim();

                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await CreatePost(blogService);
                            break;
                        case "2":
                            await ViewPostList(blogService);
                            break;
                        case "3":
                            await ViewACertainPost(blogService);
                            break;
                        case "4":
                            await UpdatePost(blogService);
                            break;
                        case "5":
                            await DeletePost(blogService);
                            break;
                        case "6":
                            await CreateUpdateDelete(blogService);
                            break;
                        case "0":
                            Console.WriteLine("\naww...See you!");
                            return;
                        default:
                            Console.WriteLine("Warning - invalid choice!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nERROR: {ex.Message}");
                    Console.WriteLine($"Type: {ex.GetType().Name}");
                }

                Console.WriteLine("\nPress any button to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // ============================================================================
        // MENU PRINCIPALE
        // ============================================================================
        static void ShowMenu()
        {
            Console.WriteLine("==================================================================");
            Console.WriteLine("                        MAIN MENU                           ");
            Console.WriteLine("==================================================================");
            Console.WriteLine("  1. Create a new post ");
            Console.WriteLine("  2. View all the posts in this blog ");
            Console.WriteLine("  3. View a certain post ");
            Console.WriteLine("  4. Update a certain post ");
            Console.WriteLine("  5. Delete a certain post ");
            Console.WriteLine("  6. Create/Upgrade/delete (CRUD) ");
            Console.WriteLine("  0. Leave :C ");
            Console.WriteLine("==================================================================");
            Console.Write("\nYour choice: ");
        }

        // ============================================================================
        // 1. CREA ARTICOLO
        // ============================================================================
        static async Task CreatePost(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("1. CREATE ARTICLE ");
            Console.WriteLine("============================================================================");

            Console.Write("Title: ");
            string title = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title can't be empty!");
                return;
            }

            Console.Write("Content: ");
            string content = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Content can't be empty!");
                return;
            }

            Console.WriteLine("\nSaving...");

            await blogService.CreateArticleAsync(new PostCreateDto(title, content));

            Console.WriteLine("Post successfully created!");
        }

        static async Task ViewPostList(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("2. VIEW ALL THE POSTS ");
            Console.WriteLine("============================================================================");

            Console.WriteLine("Loading...\n");

            var postFound = await blogService.GetAllArticlesAsync();
            var postList = postFound.ToList();

            if (!postList.Any())
            {
                Console.WriteLine("List is still empty :C .");
                return;
            }

            Console.WriteLine($"{postList.Count} posts were found:\n");
            Console.WriteLine(new string('─', 80));

            foreach (var post in postList)
            {
                Console.WriteLine($"\nID: {post.Id}");
                Console.WriteLine($"Title: {post.Title}");
                Console.WriteLine($"Date: {post.CreatedAt:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Content: {post.Content}");
                Console.WriteLine(new string('─', 80));
            }

            Console.WriteLine($"\nTotal posts: {postList.Count}");
        }

        // ============================================================================
        // 3. VISUALIZZA ARTICOLO SPECIFICO
        // ============================================================================
        static async Task ViewACertainPost(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("3. VIEW POST ");
            Console.WriteLine("============================================================================");

            Console.Write("Please write down the ID of the post: ");
            var id = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("invalid ID!");
                return;
            }

            Console.WriteLine("\nSearching for a matching post...\n");

            var post = await blogService.GetArticleByIdAsync(id);

            if (post == null)
            {
                Console.WriteLine($"Post with matching id to'{id}' not found!");
                return;
            }


            Console.WriteLine(post.Title);
            Console.WriteLine($"ID: {post.Id}");
            Console.WriteLine($"Creation date: {post.CreatedAt:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"\nContent:\n{new string('─', 80)}");
            Console.WriteLine(post.Content);
            Console.WriteLine(new string('─', 80));
        }

        // ============================================================================
        // 4. MODIFICA ARTICOLO
        // ============================================================================
        static async Task UpdatePost(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("4. UPDATE POST ");
            Console.WriteLine("============================================================================");

            Console.Write("Write down the post's ID: ");
            var id = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("invalid ID!");
                return;
            }

            Console.WriteLine("\nSearching up...\n");
            var articolo = await blogService.GetArticleByIdAsync(id);

            if (articolo == null)
            {
                Console.WriteLine($"Post with matching ID '{id}' not found!");
                return;
            }
            else
            {
                Console.WriteLine("Post successfully found!");
            }

            Console.Write("New title: ");
            string title = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title can't be empty!");
                return;
            }

            Console.Write("New content: ");
            string content = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Content can't be empty!");
                return;
            }

            Console.WriteLine("\nSaving up...");

            await blogService.UpdateArticleAsync(articolo.Id, new PostCreateDto(title, content));

            Console.WriteLine("Post successfully posted!");
        }

        // ============================================================================
        // 5. ELIMINA ARTICOLO
        // ============================================================================
        static async Task DeletePost(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("5. DELETE POST ");
            Console.WriteLine("============================================================================");

            Console.Write("Write down post's id: ");
            var id = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID not found!");
                return;
            }
            else
            {
                Console.WriteLine("ID successfully found!");
            }

            Console.WriteLine("\nSearching up...\n");
            var articolo = await blogService.GetArticleByIdAsync(id);

            if (articolo == null)
            {
                Console.WriteLine($"Post with matching id '{id}' not found!");
                return;
            }
            else
            {
                Console.WriteLine("Post successfully found!");
            }

            await blogService.DeleteArticleAsync(articolo.Id);

            Console.WriteLine("post successfully deleted!");
        }

        // ============================================================================
        // 6. TEST COMPLETO CRUD
        // ============================================================================
        static async Task CreateUpdateDelete(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("6. TEST CREATE/REVIEW/UPGRADE/DELETE (CRUD) ");
            Console.WriteLine("============================================================================");

            Console.WriteLine("============================================================================");
            Console.WriteLine(" CREATE ARTICLE ");
            Console.WriteLine("============================================================================");

            Console.Write("Title: ");
            string titleCreate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(titleCreate))
            {
                Console.WriteLine("Title can't be empty!");
                return;
            }

            Console.Write("Content: ");
            string contentCreate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(contentCreate))
            {
                Console.WriteLine("Content can't be empty!");
                return;
            }

            Console.WriteLine("\nSaving up...");

            var postCreateDto = new PostCreateDto(titleCreate, contentCreate);

            await blogService.CreateArticleAsync(postCreateDto);

            Console.WriteLine("Post successfully posted!");

            var createdPosts = await blogService.GetAllArticlesAsync();
            var post = createdPosts.FirstOrDefault(p => p.Title == titleCreate && p.Content == contentCreate);

            Console.WriteLine(post.Title);
            Console.WriteLine($"ID: {post.Id}");
            Console.WriteLine($"Creation date: {post.CreatedAt:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"\nContent:\n{new string('─', 80)}");
            Console.WriteLine(post.Content);
            Console.WriteLine(new string('─', 80));

            Console.WriteLine("============================================================================");
            Console.WriteLine(" UPDATE POST ");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var idUpdate = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(idUpdate))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nSearching up...\n");
            var postUpdate = await blogService.GetArticleByIdAsync(idUpdate);

            if (postUpdate == null)
            {
                Console.WriteLine($"Post with matching id '{idUpdate}' not found!");
                return;
            }
            else
            {
                Console.WriteLine("Post successfully found!");
            }

            Console.Write("New title: ");
            string titleUpdate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(titleUpdate))
            {
                Console.WriteLine("Title can't be empty!");
                return;
            }

            Console.Write("New content: ");
            string contentUpdate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(contentUpdate))
            {
                Console.WriteLine("Content can't be empty!");
                return;
            }

            Console.WriteLine("\nSaving up...");

            await blogService.UpdateArticleAsync(postUpdate.Id, new PostCreateDto(titleUpdate, contentUpdate));

            Console.WriteLine("Post  successfully updated!");

            Console.WriteLine("============================================================================");
            Console.WriteLine(" DELETE POST ");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var idDelete = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(idDelete))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nSearching up...\n");
            var articoloDelete = await blogService.GetArticleByIdAsync(idDelete);

            if (articoloDelete == null)
            {
                Console.WriteLine($"Post with matching id '{idDelete}' not found!");
                return;
            }
            else
            {
                Console.WriteLine("Post successfully found!");
            }

            await blogService.DeleteArticleAsync(articoloDelete.Id);

            Console.WriteLine("Post successfully deleted!");
        }
    }
}
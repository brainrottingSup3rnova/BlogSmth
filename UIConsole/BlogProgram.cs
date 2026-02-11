using Application.Interfaces;   
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Application.UseCases;
using Infrastructure.Repositories;
using Application.Dto;


namespace BlogConsole
{
    internal class BlogProgram
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Blog Manager...");

            // Configurazione del Generic Host con Dependency Injection
            using IHost host = Host.CreateDefaultBuilder(args) //lo using garantisce che il host venga correttamente "smaltito" alla fine del blocco in automatico
                .ConfigureServices((context, services) =>
                {
                    // ===== REGISTRAZIONE CLEAN ARCHITECTURE LAYERS =====
                    // Infrastructure Layer - Json
                    services.AddScoped<IBlogRepository>(sp => //aggiunge un service, creandone un'istanza
                        new JsonBlogRepository());

                    // Application Layer
                    services.AddScoped<IBlogService, BlogService>();
                })
                .Build();

            Console.WriteLine("Dependency Injection configurata con successo!\n");
            Console.WriteLine("Premi un tasto per iniziare...");
            Console.ReadKey();
            Console.Clear();

            // Avvio dell'applicazione
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
                MostraMenu();

                var scelta = Console.ReadLine()?.Trim();

                Console.WriteLine();

                try
                {
                    switch (scelta)
                    {
                        case "1":
                            await CreaArticolo(blogService);
                            break;
                        case "2":
                            await ListaArticoli(blogService);
                            break;
                        case "3":
                            await VisualizzaArticolo(blogService);
                            break;
                        case "4":
                            await ModificaArticolo(blogService);
                            break;
                        case "5":
                            await EliminaArticolo(blogService);
                            break;
                        case "6":
                            await TestCompleto(blogService);
                            break;
                        case "0":
                            Console.WriteLine("\nArrivederci!");
                            return;
                        default:
                            Console.WriteLine("Attenzione - Scelta non valida!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nERRORE: {ex.Message}");
                    Console.WriteLine($"Tipo: {ex.GetType().Name}");
                }

                Console.WriteLine("\nPremi un tasto per continuare...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // ============================================================================
        // MENU PRINCIPALE
        // ============================================================================
        static void MostraMenu()
        {
            Console.WriteLine("==================================================================");
            Console.WriteLine("                        MENU PRINCIPALE                           ");
            Console.WriteLine("==================================================================");
            Console.WriteLine("  1. Crea Nuovo Articolo ");
            Console.WriteLine("  2. Lista Tutti gli Articoli ");
            Console.WriteLine("  3. Visualizza Articolo Specifico ");
            Console.WriteLine("  4. Modifica Articolo ");
            Console.WriteLine("  5. Elimina Articolo ");
            Console.WriteLine("  6. Test Completo (CRUD) ");
            Console.WriteLine("  0. Esci ");
            Console.WriteLine("==================================================================");
            Console.Write("\nScelta: ");
        }

        // ============================================================================
        // 1. CREA ARTICOLO
        // ============================================================================
        static async Task CreaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("1. CREA ARTICOLO ");
            Console.WriteLine("============================================================================");

            Console.Write("Titolo: ");
            string title = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Il titolo non può essere vuoto!");
                return;
            }

            Console.Write("Contenuto: ");
            string content = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Il contenuto non può essere vuoto!");
                return;
            }

            Console.WriteLine("\nSalvataggio in corso...");

            await blogService.CreateArticleAsync(new PostCreateDto(title, content));

            Console.WriteLine("Articolo pubblicato con successo!");
        }

        // ============================================================================
        // 2. LISTA ARTICOLI
        // ============================================================================
        static async Task ListaArticoli(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("2. LISTA ARTICOLI ");
            Console.WriteLine("============================================================================");

            Console.WriteLine("Caricamento...\n");

            var articoli = await blogService.GetAllArticlesAsync();
            var listaArticoli = articoli.ToList();

            if (!listaArticoli.Any())
            {
                Console.WriteLine("Nessun articolo trovato nel database.");
                return;
            }

            Console.WriteLine($"Trovati {listaArticoli.Count} articoli:\n");
            Console.WriteLine(new string('─', 80));

            foreach (var articolo in listaArticoli)
            {
                Console.WriteLine($"\nID: {articolo.Id}");
                Console.WriteLine($"Titolo: {articolo.Title}");
                Console.WriteLine($"Data: {articolo.CreatedAt:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Content: {articolo.Content}");
                Console.WriteLine(new string('─', 80));
            }

            Console.WriteLine($"\nTotale articoli: {listaArticoli.Count}");
        }

        // ============================================================================
        // 3. VISUALIZZA ARTICOLO SPECIFICO
        // ============================================================================
        static async Task VisualizzaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("3. VISUALIZZA ARTICOLO SPECIFICO ");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var id = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nRicerca in corso...\n");

            var articolo = await blogService.GetArticleByIdAsync(id);

            if (articolo == null)
            {
                Console.WriteLine($"Articolo con ID '{id}' non trovato!");
                return;
            }


            Console.WriteLine(articolo.Title);
            Console.WriteLine($"ID: {articolo.Id}");
            Console.WriteLine($"Data Creazione: {articolo.CreatedAt:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"\nCONTENUTO:\n{new string('─', 80)}");
            Console.WriteLine(articolo.Content);
            Console.WriteLine(new string('─', 80));
        }

        // ============================================================================
        // 4. MODIFICA ARTICOLO
        // ============================================================================
        static async Task ModificaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("4. MODIFICA UN ARTICOLO SPECIFICO");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var id = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nRicerca in corso...\n");
            var articolo = await blogService.GetArticleByIdAsync(id);

            if (articolo == null)
            {
                Console.WriteLine($"Articolo con ID '{id}' non trovato!");
                return;
            }
            else
            {
                Console.WriteLine("Articolo trovato con successo!");
            }

            Console.Write("Nuovo titolo: ");
            string title = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Il titolo non può essere vuoto!");
                return;
            }

            Console.Write("Nuovo contenuto: ");
            string content = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Il contenuto non può essere vuoto!");
                return;
            }

            Console.WriteLine("\nSalvataggio in corso...");

            await blogService.UpdateArticleAsync(articolo.Id, new PostCreateDto(title, content));

            Console.WriteLine("Articolo pubblicato con successo!");
        }

        // ============================================================================
        // 5. ELIMINA ARTICOLO
        // ============================================================================
        static async Task EliminaArticolo(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("5. ELIMINA UN ARTICOLO SPECIFICO");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var id = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nRicerca in corso...\n");
            var articolo = await blogService.GetArticleByIdAsync(id);

            if (articolo == null)
            {
                Console.WriteLine($"Articolo con ID '{id}' non trovato!");
                return;
            }
            else
            {
                Console.WriteLine("Articolo trovato con successo!");
            }

            await blogService.DeleteArticleAsync(articolo.Id);

            Console.WriteLine("Articolo eliminato con successo!");
        }

        // ============================================================================
        // 6. TEST COMPLETO CRUD
        // ============================================================================
        static async Task TestCompleto(IBlogService blogService)
        {
            Console.WriteLine("============================================================================");
            Console.WriteLine("6. CRUD TESTO COMPLETO");
            Console.WriteLine("============================================================================");

            Console.WriteLine("============================================================================");
            Console.WriteLine("CREA UN NUOVO ARTICOLO");
            Console.WriteLine("============================================================================");

            Console.Write("Titolo: ");
            string titleCreate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(titleCreate))
            {
                Console.WriteLine("Il titolo non può essere vuoto!");
                return;
            }

            Console.Write("Contenuto: ");
            string contentCreate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(contentCreate))
            {
                Console.WriteLine("Il contenuto non può essere vuoto!");
                return;
            }

            Console.WriteLine("\nSalvataggio in corso...");

            await blogService.CreateArticleAsync(new PostCreateDto(titleCreate, contentCreate));

            Console.WriteLine("Articolo pubblicato con successo!");

            Console.WriteLine("============================================================================");
            Console.WriteLine("5. MODIFICA UN ARTICOLO");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var idUpdate = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(idUpdate))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nRicerca in corso...\n");
            var articoloUpdate = await blogService.GetArticleByIdAsync(idUpdate);

            if (articoloUpdate == null)
            {
                Console.WriteLine($"Articolo con ID '{idUpdate}' non trovato!");
                return;
            }
            else
            {
                Console.WriteLine("Articolo trovato con successo!");
            }

            Console.Write("Nuovo titolo: ");
            string titleUpdate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(titleUpdate))
            {
                Console.WriteLine("Il titolo non può essere vuoto!");
                return;
            }

            Console.Write("Nuovo contenuto: ");
            string contentUpdate = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(contentUpdate))
            {
                Console.WriteLine("Il contenuto non può essere vuoto!");
                return;
            }

            Console.WriteLine("\nSalvataggio in corso...");

            await blogService.UpdateArticleAsync(articoloUpdate.Id, new PostCreateDto(titleUpdate, contentUpdate));

            Console.WriteLine("Articolo pubblicato con successo!");

            Console.WriteLine("============================================================================");
            Console.WriteLine("5. ELIMINA UN ARTICOLO SPECIFICO");
            Console.WriteLine("============================================================================");

            Console.Write("Inserisci l'ID dell'articolo: ");
            var idDelete = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(idDelete))
            {
                Console.WriteLine("ID non valido!");
                return;
            }

            Console.WriteLine("\nRicerca in corso...\n");
            var articoloDelete = await blogService.GetArticleByIdAsync(idDelete);

            if (articoloDelete == null)
            {
                Console.WriteLine($"Articolo con ID '{idDelete}' non trovato!");
                return;
            }
            else
            {
                Console.WriteLine("Articolo trovato con successo!");
            }

            await blogService.DeleteArticleAsync(articoloDelete.Id);

            Console.WriteLine("Articolo eliminato con successo!");
        }
    }
}
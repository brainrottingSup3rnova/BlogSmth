using Application.Dto;
using Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BlogWpf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWpf.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IBlogService _blogService;

        // ===== OBSERVABLE PROPERTIES =====
        // Utilizzo di ObservableCollection per aggiornamenti automatici della UI quando la collezione cambia
        [ObservableProperty]
        private ObservableCollection<PostReadDto> _articles = new();

        // Proprieta' per la gestione dell'articolo selezionato e dei nuovi dati per la creazione
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsArticleSelected))]
        private PostReadDto? _selectedArticle;

        // Campi per la creazione di un nuovo articolo, con notifiche per abilitare/disabilitare il comando di creazione
        [ObservableProperty]
        private string _newTitle = string.Empty;

        [ObservableProperty]
        private string _newContent = string.Empty;

        // Stato di caricamento e messaggi di status per feedback all'utente
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        // Property computed per verificare se un articolo è selezionato, usata per abilitare/disabilitare i comandi di aggiornamento e cancellazione
        public bool IsArticleSelected => SelectedArticle != null;

        // ===== CONSTRUCTOR =====
        // Iniezione del servizio e caricamento iniziale degli articoli
        public HomeViewModel(IBlogService blogService)
        {
            _blogService = blogService;

            // Carica articoli all'avvio
            _ = LoadArticlesAsync();
        }

        // Caricamento articoli con gestione dello stato di caricamento e messaggi di status
        [RelayCommand]
        private async Task LoadArticlesAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Caricamento articoli...";

                var articles = await _blogService.GetAllArticlesAsync();

                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }

                StatusMessage = $"Caricati {Articles.Count} articoli";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }

        }
    }
}

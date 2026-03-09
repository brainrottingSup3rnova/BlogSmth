using Accessibility;
using Application.Dto;
using Application.Interfaces;
using BlogWpf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlogWpf.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IBlogService _blogService;

        [ObservableProperty]
        private ObservableCollection<PostReadDto> _articles = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsArticleSelected))]
        private PostReadDto? _selectedArticle;

        [ObservableProperty]
        private string _newTitle = string.Empty;

        [ObservableProperty]
        private string _newContent = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public bool IsArticleSelected => SelectedArticle != null;

        public HomeViewModel(IBlogService blogService)
        {
            _blogService = blogService;

            // Carica articoli all'avvio
            _ = LoadArticlesAsync();
        }

        [RelayCommand]
        private async Task LoadArticlesAsync()
        {
            try
            {
                var articles = await _blogService.GetAllArticlesAsync();

                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }

                StatusMessage = $"Loaded {Articles.Count} articles";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task CreateArticleAsync()
        {
            CreatePostView createPostView = new CreatePostView(new CreatePostViewModel(_blogService));
            createPostView.ShowDialog();
        }

        partial void OnSelectedArticleChanged(PostReadDto? post)
        {
            if (post == null)
                return;
            SeePostWindow seePostWindow = new SeePostWindow(post, new SeePostViewModel(_blogService, post));
            seePostWindow.ShowDialog();
        }
    }
}

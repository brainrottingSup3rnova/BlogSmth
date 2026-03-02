using Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlogWpf.ViewModels
{
    public partial class CreatePostViewModel:ObservableObject
    {
        private IBlogService _blogService;

        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _content = string.Empty;

        public CreatePostViewModel(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [RelayCommand]
        private async Task CreatePost()
        {
            try
            {
                await _blogService.CreateArticleAsync(new Application.Dto.PostCreateDto(Title, Content));
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

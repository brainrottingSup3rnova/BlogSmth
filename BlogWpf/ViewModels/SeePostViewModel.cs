using Application.Dto;
using Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace BlogWpf.ViewModels
{
    public partial class SeePostViewModel : ObservableObject
    {
        private IBlogService _blogService;

        [ObservableProperty]
        private PostReadDto _post;
        [ObservableProperty]
        private string _editedTitle = string.Empty;
        [ObservableProperty]
        private string _editedContent = string.Empty;

        public SeePostViewModel(IBlogService blogService, PostReadDto postDto)
        {
            _blogService = blogService;
            _post = postDto;
        }

        [RelayCommand]
        private async Task EditPost()
        {
            try
            {
                await _blogService.UpdateArticleAsync(_post.Id, new PostCreateDto( _post.Title,_post.Content));
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante la cancellazione del post: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task DeletePost()
        {
            try
            {
                await _blogService.DeleteArticleAsync(_post.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante la cancellazione del post: {ex.Message}");
            }
        }
    }
}

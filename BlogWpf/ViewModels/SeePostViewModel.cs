using Application.Dto;
using Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

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
            EditedContent = postDto.Content;
            EditedTitle = postDto.Title;
        }

        [RelayCommand]
        private async Task DeletePostAsync()
        {
            try
            {
                await _blogService.DeleteArticleAsync(_post.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task UpdatePostAsync()
        {
            try
            {
                await _blogService.UpdateArticleAsync(_post.Id, new PostCreateDto(EditedTitle, EditedContent));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

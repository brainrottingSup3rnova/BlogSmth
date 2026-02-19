using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlogWpf.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IBlogService _blogService;

        [ObservableProperty]
        private ObservableCollection<PostReadDto> _articles = new();

        [ObservableProperty]
        private PostReadDto? _selectedArticle;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _content = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;
    }
}

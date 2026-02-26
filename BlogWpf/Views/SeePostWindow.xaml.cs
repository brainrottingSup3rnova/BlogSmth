using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BlogWpf.ViewModels;
using Application.Dto;
using System.Windows.Media.TextFormatting;

namespace BlogWpf.Views
{
    /// <summary>
    /// Logica di interazione per SeePostWpf.xaml
    /// </summary>
    public partial class SeePostWindow : Window
    {
        private readonly PostReadDto _post;
        public SeePostWindow(PostReadDto post, SeePostViewModel viewModel)
        {
            InitializeComponent();
            _post = post;
            DataContext = viewModel;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

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
using BlogWpf.ViewModels;
using System.Windows.Shapes;

namespace BlogWpf.Views
{
    /// <summary>
    /// Logica di interazione per CreatePostView.xaml
    /// </summary>
    public partial class CreatePostView : Window
    {
        public CreatePostView(CreatePostViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

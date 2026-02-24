using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlogWpf.ViewModels;

namespace BlogWpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(HomeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ArticlesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gestione della selezione dell'articolo, se necessario
        }
    }
}
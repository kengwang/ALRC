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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ALRC.Creator.Models.ViewModels;

namespace ALRC.Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; set; }
        
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            InitializeComponent();
            
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ViewModel.RootNavigation = RootNavigation;
            RootFrameContainer.Children.Clear();
            RootFrameContainer.Children.Add(ViewModel.RootFrame);
            
            PlayControllerContainer.Children.Clear();
            PlayControllerContainer.Children.Add(ViewModel.PlayController);
            
            ViewModel.NavigateToLyricInfoPage();
        }

        private void Nav_Home_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToHome();
        }

        private void Nav_LyricInfo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToLyricInfoPage();
        }

        private void Nav_Settings_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToSettings();
        }

        private void Nav_LinesEdit_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToLinesEdit();
        }

        private void Nav_StylesEdit_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToStylesEdit();
        }

        private void Nav_WordEdit_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToWordEdit();
        }

        private void Nav_Export_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToExport();
        }

        private void Nav_Convert_Click(object sender, RoutedEventArgs e)
        {
                ViewModel.NavigateToConvert();
        }

        private void Nav_Preview_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToPreview();
        }
    }
}
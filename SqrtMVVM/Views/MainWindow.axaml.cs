using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SqrtMVVM.ViewModels;
using System;

namespace SqrtMVVM.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCellPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            var cell = border.DataContext as MainWindowViewModel.CheckerCell;
            if (cell == null) return;

            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel == null) return;

            // Обрабатываем клик через ViewModel
            viewModel.OnCellClick(cell);
            e.Handled = true;
        }
    }
}
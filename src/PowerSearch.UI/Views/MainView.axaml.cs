using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PowerSearch.ViewModels;

namespace PowerSearch.Views;

public partial class MainView : UserControl
{
    private MainViewModel? _vm;

    public MainView()
    {
        InitializeComponent();
    }

    private async void btnLoadProfile_Click(object? sender, RoutedEventArgs e)
    {
        var top = TopLevel.GetTopLevel(this);
        var files = await top.StorageProvider.OpenFilePickerAsync(new()
        {
            FileTypeFilter = [
        new FilePickerFileType("Search Profile")
                {
                    Patterns = ["*.json"],
                },
            ],
        });

        using var stream = await files[0].OpenReadAsync();
        _vm.LoadProfile(stream);
    }

    private void btnNewProfile_Click(object? sender, RoutedEventArgs e)
    {
        //todo: ask to save
        _vm.NewProfile();
    }

    private async void btnSaveProfile_Click(object? sender, RoutedEventArgs e)
    {
        var top = TopLevel.GetTopLevel(this);
        var file = await top.StorageProvider.SaveFilePickerAsync(new()
        {
            DefaultExtension = ".json",
            FileTypeChoices = [
                new FilePickerFileType("Search Profile")
                {
                    Patterns = ["*.json"],
                },
            ],
            ShowOverwritePrompt = true,
        });

        using var stream = await file.OpenWriteAsync();
        _vm.SaveProfile(stream);
    }

    private void MainView_Loaded(object? sender, RoutedEventArgs e)
    {
        _vm = (MainViewModel)DataContext;
    }
}

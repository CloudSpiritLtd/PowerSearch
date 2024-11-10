using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using DryIoc.ImTools;
using DynamicData;
using MsBox.Avalonia;
using Newtonsoft.Json;
using PowerSearch.Models;
using PowerSearch.Runner;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PowerSearch.ViewModels;

public class MainViewModel : ViewModelBase
{
    private Profile _profile = new();

    public MainViewModel()
    {
#if DEBUG
        if (Design.IsDesignMode)
        {
            AddPipelineItem();
            Results.Add(new() { FileName = "Demo", Text = "Demo Text" });
        }
#endif

        AddPipelineItemCommand = ReactiveCommand.Create(AddPipelineItem);
        DeletePipelineItemCommand = ReactiveCommand.Create<PipelineItemViewModel>(DeletePipelineItem);
        SearchCommand = ReactiveCommand.Create(Search);
    }

    public void AddPipelineItem()
    {
        PipelineItem item = new();
        _profile.Pipeline.Add(item);
        Pipeline.Add(new PipelineItemViewModel(item));
    }

    public void DeletePipelineItem(PipelineItemViewModel cond)
    {
        Pipeline.Remove(cond);
    }

    public void LoadProfile(Stream file)
    {
        var profile = Profile.LoadFromFile(file);
        if (profile == null)
        {
            throw new InvalidDataException("Invalid profile");
        }
        else
        {
            //todo: need release _profile?

            _profile = profile;
            LoadSettingsFromProfile();
        }
    }

    public void NewProfile()
    {
        //todo: need release _profile?

        _profile = new Profile();
        LoadSettingsFromProfile();
    }

    public void SaveProfile(Stream stream)
    {
        Profile.SaveToFile(stream, _profile);
    }

    public async void Search()
    {
        var dir = Path.GetDirectoryName(SearchIn);
        if (string.IsNullOrEmpty(dir))
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", "You must provide a valid path to search in.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await box.ShowAsync();
            return;
        }

        if (_profile.Pipeline.Count == 0)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", "No search conditions.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            await box.ShowAsync();
            return;
        }

        Results.Clear();
        SimpleRunner runner = new(_profile);
        var results = runner.Run(SearchIn);
        await foreach (var result in results)
        {
            Results.Add(result);
        }
    }

    private void LoadSettingsFromProfile()
    {
        Pipeline.Clear();
        _profile.Pipeline.ForEach(cond => Pipeline.Add(new PipelineItemViewModel(cond)));
    }

    public ReactiveCommand<Unit, Unit> AddPipelineItemCommand { get; }

    public ObservableCollection<PipelineItemViewModel> Pipeline { get; set; } = [];

    public ReactiveCommand<PipelineItemViewModel, Unit> DeletePipelineItemCommand { get; }

    public ObservableCollection<SearchResult> Results { get; set; } = [];

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    public string SearchIn { get; set; } = string.Empty;
}

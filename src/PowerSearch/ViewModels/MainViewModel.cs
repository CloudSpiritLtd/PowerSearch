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
    private SearchProfile _profile = new();

    public MainViewModel()
    {
#if DEBUG
        if (Design.IsDesignMode)
        {
            AddCondition();
            Results.Add(new() { FileName = "Demo", Text = "Demo Text" });
        }
#endif

        AddConditionCommand = ReactiveCommand.Create(AddCondition);
        DeleteConditionCommand = ReactiveCommand.Create<ConditionViewModel>(DeleteCondition);
        SearchCommand = ReactiveCommand.Create(Search);
    }

    public void AddCondition()
    {
        Condition cond = new();
        _profile.Conditions.Add(cond);
        Conditions.Add(new ConditionViewModel(cond));
    }

    public void DeleteCondition(ConditionViewModel cond)
    {
        Conditions.Remove(cond);
    }

    public void LoadProfile(Stream file)
    {
        var profile = SearchProfile.LoadFromFile(file);
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

        _profile = new SearchProfile();
        LoadSettingsFromProfile();
    }

    public void SaveProfile(Stream stream)
    {
        SearchProfile.SaveToFile(stream, _profile);
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

        if (_profile.Conditions.Count == 0)
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
        Conditions.Clear();
        _profile.Conditions.ForEach(cond => Conditions.Add(new ConditionViewModel(cond)));
    }

    public ReactiveCommand<Unit, Unit> AddConditionCommand { get; }

    public ObservableCollection<ConditionViewModel> Conditions { get; set; } = [];

    public ReactiveCommand<ConditionViewModel, Unit> DeleteConditionCommand { get; }

    public ObservableCollection<SearchResult> Results { get; set; } = [];

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    public string SearchIn { get; set; } = string.Empty;
}

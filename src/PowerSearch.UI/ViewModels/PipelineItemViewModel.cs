using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSearch.Models;
using ReactiveUI;

namespace PowerSearch.ViewModels;

public class PipelineItemViewModel : ViewModelBase
{
    private readonly PipelineItem _item;

    public PipelineItemViewModel(PipelineItem item)
    {
        _item = item;
        UseExtract = !_item.Extract.IsEmpty();
    }

    public Search Search { get => _item.Search; }

    public Extract Extract { get => _item.Extract; }


    public bool UseExtract
    {
        get { return _item.UseExtract; }
        set 
        {
            if (value != _item.UseExtract)
            {
                _item.UseExtract = value;
                (this as IReactiveObject).RaisePropertyChanged();
            }
        }
    }
}

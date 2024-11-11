using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSearch.Models;
using ReactiveUI.Fody.Helpers;

namespace PowerSearch.ViewModels;

public class PipelineItemViewModel(PipelineItem item) : ViewModelBase
{
    private readonly PipelineItem _item = item;
    private readonly Extract? _extract;
    private bool _useExtract;

    public PipelineItem Target { get => _item; }

    [Reactive]
    public bool UseExtract
    {
        get { return _useExtract; }
        set 
        {
            if (value != _useExtract)
            {
                _useExtract = value;
                _item.Extract = value ? _extract : null;
            }
        }
    }
}

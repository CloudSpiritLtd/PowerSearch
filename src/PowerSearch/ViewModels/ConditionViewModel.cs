using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSearch.Models;
using ReactiveUI.Fody.Helpers;

namespace PowerSearch.ViewModels;

public class ConditionViewModel : ViewModelBase
{
    private readonly Condition _condition;
    private readonly Extract _extract;
    private bool _useExtract;

    public ConditionViewModel(Condition condition)
    {
        _condition = condition;
    }

    public Condition Target { get => _condition; }

    [Reactive]
    public bool UseExtract
    {
        get { return _useExtract; }
        set 
        {
            if (value != _useExtract)
            {
                _useExtract = value;
                _condition.Extract = value ? _extract : null;
            }
        }
    }
}

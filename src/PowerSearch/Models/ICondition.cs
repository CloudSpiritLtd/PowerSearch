using System.Text.RegularExpressions;

namespace PowerSearch.Models;

public interface ICondition
{
    SearchKind Kind { get; set; }

    string Expression { get; set; }

    bool IgnoreCase { get; set; }

    IExtract? Extract { get; set; }
}

public interface IExtract
{
    int Match { get; set; }
    int Group { get; set; }
}

public enum SearchKind
{
    Text,
    Wildcard,
    Regex,
}

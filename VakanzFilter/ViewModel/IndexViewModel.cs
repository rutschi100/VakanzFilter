using System.Text.RegularExpressions;
using VakanzFilter.Data;
using VakanzFilter.Data.Entities;
using VakanzFilter.Services;

namespace VakanzFilter.ViewModel;

public class IndexViewModel
{
    private readonly IDataService _dataService;

    public Filters filters { get; set; }
    public string maybeText = string.Empty;
    public string goodText = string.Empty;
    public string noGoText = string.Empty;
    public string vacancyText = string.Empty;
    public List<FilterResults> scanResults = new();

    public IndexViewModel(IDataService dataService)
    {
        _dataService = dataService;
        filters = _dataService.LoadFilters() ?? new();
    }

    public void AddFilter(FilterType filterType)
    {
        switch (filterType)
        {
            case FilterType.NoGo:
                noGoText = noGoText.Trim().ToLower();
                if (!IsFilterValid(noGoText))
                {
                    break;
                }

                filters.NoGo.Add(noGoText);
                noGoText = string.Empty;
                break;
            case FilterType.Maybe:
                maybeText = maybeText.Trim().ToLower();
                if (!IsFilterValid(maybeText))
                {
                    break;
                }

                filters.Maybe.Add(maybeText);
                maybeText = string.Empty;
                break;
            case FilterType.Good:
                goodText = goodText.Trim().ToLower();
                if (!IsFilterValid(goodText))
                {
                    break;
                }

                filters.Good.Add(goodText);
                goodText = string.Empty;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null);
        }

        _dataService.SaveFilters(filters);
    }

    private bool IsFilterValid(string newFilter)
    {
        if (string.IsNullOrWhiteSpace(newFilter))
        {
            return false;
        }

        if (
            filters.Good.Any(s => s == newFilter)
         || filters.Maybe.Any(s => s == newFilter)
         || filters.NoGo.Any(s => s == newFilter)
        )
        {
            return false;
        }

        return true;
    }

    public void DeleteFilter(FilterType filterType, string oneFilter)
    {
        switch (filterType)
        {
            case FilterType.NoGo:
                filters.NoGo.Remove(oneFilter);
                break;
            case FilterType.Maybe:
                filters.Maybe.Remove(oneFilter);
                break;
            case FilterType.Good:
                filters.Good.Remove(oneFilter);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null);
        }

        _dataService.SaveFilters(filters);
    }


    public void ScanVacancyText()
    {
        scanResults = new List<FilterResults>();
        var filterList = filters.NoGo;
        var filterType = FilterType.NoGo;

        FindFilterInText(filterList, filterType, scanResults);

        filterList = filters.Maybe;
        filterType = FilterType.Maybe;

        FindFilterInText(filterList, filterType, scanResults);

        filterList = filters.Good;
        filterType = FilterType.Good;

        FindFilterInText(filterList, filterType, scanResults);
    }

    private void FindFilterInText(
        List<string> filterList,
        FilterType filterType,
        List<FilterResults> founded
    )
    {
        if (string.IsNullOrWhiteSpace(vacancyText))
        {
            return;
        }
        var lastChar = vacancyText[^1];
        if (lastChar != '\n')
        {
            vacancyText += "\n";
        }

        founded.AddRange(
            from oneFilter in filterList
            let regex = new Regex($".*{oneFilter}.*\n", RegexOptions.IgnoreCase)
            let match = regex.Match(vacancyText)
            
            let index = match.Value.IndexOf(oneFilter, StringComparison.OrdinalIgnoreCase)
            let textBeforeFilter = index >= 0 ? match.Value.Substring(0, index) : string.Empty
            let textAfterFilter = index >= 0 ? match.Value.Substring(index + oneFilter.Length) : string.Empty
                                  
            where match.Success
            select new FilterResults
            {
                Filter = oneFilter,
                FilterType = filterType,
                FullContext = match.Value,
                TextBeforFilter = textBeforeFilter,
                TextAfterFilter = textAfterFilter
            }
        );
    }
}
using System.Text.RegularExpressions;
using VakanzFilter.Data;
using VakanzFilter.Services;

namespace VakanzFilter.ViewModel;

public class IndexViewModel
{
    /*  TODO
     *  - Durchsuchen von Text
     *  - Anzeigen von Gefundenen Filtern in dem Resultat bereich
     *  - Link auf die Filter Wörter setzen
     *  - Anzeigen von dem Kontext
     */

    private readonly IDataService _dataService;

    public Filters filters { get; set; } = new();

    public string maybeText = string.Empty;
    public string goodText = string.Empty;
    public string noGoText = string.Empty;
    public string vacancyText = string.Empty;
    public List<FilterResults> scanResults = new();

    public IndexViewModel(IDataService dataService)
    {
        _dataService = dataService;
        filters = _dataService.LoadFilters() ?? new ();
    }

    public void AddFilter(FilterType filterType)
    {
        switch (filterType)
        {
            case FilterType.NoGo:
                if (!IsFilterValid(noGoText))
                {
                    break;
                }

                filters.NoGo.Add(noGoText);
                noGoText = string.Empty;
                break;
            case FilterType.Maybe:
                if (!IsFilterValid(maybeText))
                {
                    break;
                }

                filters.Maybe.Add(maybeText);
                maybeText = string.Empty;
                break;
            case FilterType.Good:
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
        //Filter suchen im Text
        // Gruppieren in Kathegorien
        var founded = FindAllFilterInText();
    }

    private List<FilterResults> FindAllFilterInText()
    {
        scanResults = new List<FilterResults>();
        var filterList = filters.NoGo;
        var filterType = FilterType.NoGo;

        FindFilterInText(filterList, filterType, scanResults);

        if (scanResults.Any())
        {
            return scanResults;
        }

        filterList = filters.Maybe;
        filterType = FilterType.Maybe;

        FindFilterInText(filterList, filterType, scanResults);

        filterList = filters.Good;
        filterType = FilterType.Good;

        FindFilterInText(filterList, filterType, scanResults);
        return scanResults;
    }

    private void FindFilterInText(List<string> filterList, FilterType filterType, List<FilterResults> founded)
    {
        
        //todo: funktioniert nicht! Neu schreiben.
        founded.AddRange(
            from oneFilter in filterList
            let regex = new Regex($"^.*{oneFilter}.*$", RegexOptions.CultureInvariant)
            let match = regex.Match(vacancyText)
            where match.Success
            select new FilterResults
            {
                Filter = oneFilter,
                FilterType = filterType,
                Context = match.Value
            }
        );
    }
    
}
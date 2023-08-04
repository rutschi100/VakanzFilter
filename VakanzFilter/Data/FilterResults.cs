namespace VakanzFilter.Data;

public class FilterResults
{
    public string Filter { get; set; }
    public FilterType FilterType { get; set; }
    public string FullContext { get; set; }
    public string TextBeforFilter { get; set; }
    public string TextAfterFilter { get; set; }
}
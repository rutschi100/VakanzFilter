using System.Text.Json;
using VakanzFilter.Data;
using VakanzFilter.Data.Entities;

namespace VakanzFilter.Services;

public interface IDataService
{
    void SaveFilters(Filters filters);
    Filters? LoadFilters();
}

public class DataService : IDataService
{
    private const string FILENAME = "Filterlist.json";

    public void SaveFilters(Filters filters)
    {
        var json = JsonSerializer.Serialize(filters);

        var writer = new StreamWriter(FILENAME, false);
        writer.Write(json);
        writer.Close();
    }

    public Filters? LoadFilters()
    {
        var json = File.ReadAllText(FILENAME);

        var filters = JsonSerializer.Deserialize<Filters>(json);

        return filters;
    }
}
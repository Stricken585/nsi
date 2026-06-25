using System.Text.Json;
using System.Text.Json.Serialization;
using NHibernate;
using nsi.Models;
using ISession = NHibernate.ISession;

namespace nsi.Services;

public class NsiApiService
{
    private readonly HttpClient _http;
    private readonly ISession _db;

    private const string UserKey = "f0bd2dfa-ae96-4a3c-9093-0431a246cf8b";
    private const string Identifier = "1.2.643.5.1.13.13.99.2.1245";
    private const string BaseUrl = "https://nsi.rosminzdrav.ru/port/rest/data";

    public NsiApiService(HttpClient http, ISession db)
    {
        _http = http;
        _db = db;
    }

    public async Task<int> FetchAndSaveAsync()
    {
        int page = 1;
        const int pageSize = 100;
        int savedCount = 0;

        while (true)
        {
            var url = $"{BaseUrl}?identifier={Identifier}&userKey={UserKey}&page={page}&size={pageSize}";
            var response = await _http.GetStringAsync(url);

            var apiResponse = JsonSerializer.Deserialize<NsiResponse>(response);
            if (apiResponse?.List == null || apiResponse.List.Count == 0)
                break;

            using var tx = _db.BeginTransaction();
            foreach (var row in apiResponse.List)
            {
                var dict = row.ToDictionary(x => x.Column, x => x.Value);

                var org = new MedicalOrganization
                {
                    Code = dict.GetValueOrDefault("code") ?? string.Empty,
                    NameFull = dict.GetValueOrDefault("nameFull") ?? string.Empty,
                    NameShort = dict.GetValueOrDefault("nameShort") ?? string.Empty,
                    Region = dict.GetValueOrDefault("region") ?? string.Empty,
                    EoOid = dict.GetValueOrDefault("eoOid") ?? string.Empty,
                    BeginDate = ParseDate(dict.GetValueOrDefault("beginDate")),
                    EndDate = ParseDate(dict.GetValueOrDefault("endDate")),
                };

                _db.Save(org);
                savedCount++;
            }
            await tx.CommitAsync();

            if (page * pageSize >= apiResponse.Total)
                break;

            page++;
        }

        return savedCount;
    }

    private static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        if (DateTime.TryParseExact(value, "dd.MM.yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var date))
            return date;
        return null;
    }
}

public class NsiResponse
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("list")]
    public List<List<NsiColumn>> List { get; set; } = [];
}

public class NsiColumn
{
    [JsonPropertyName("column")]
    public string Column { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

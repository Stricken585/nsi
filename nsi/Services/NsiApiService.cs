using System.Text.Json;
using System.Text.Json.Serialization;
using nsi.Models;
using ISession = NHibernate.ISession;

namespace nsi.Services;

public class NsiApiService
{
    private readonly HttpClient _http;
    private readonly ISession _db;
    private readonly string _userKey;
    private readonly string _identifier;
    private readonly string _baseUrl;

    private const int RetryCount = 5;

    public NsiApiService(HttpClient http, ISession db, IConfiguration config)
    {
        _http = http;
        _db = db;
        _userKey = config["Api:UserKey"]!;
        _identifier = config["Api:Identifier"]!;
        _baseUrl = config["Api:BaseUrl"]!;
    }

    public async Task<int> FetchAndSaveAsync()
    {
        int page = 1;
        const int pageSize = 100;
        int savedCount = 0;

        var apiResponse = await FetchPageAsync(page, pageSize);
        while (apiResponse != null)
        {
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
            apiResponse = await FetchPageAsync(page, pageSize);
        }

        return savedCount;
    }

    private async Task<NsiResponse?> FetchPageAsync(int page, int pageSize)
    {
        var url = $"{_baseUrl}?identifier={_identifier}&userKey={_userKey}&page={page}&size={pageSize}";
        for (int attempt = 1; attempt <= RetryCount; attempt++)
        {
            try
            {  
                var response = await _http.GetStringAsync(url);
                // Console.WriteLine($"Response: {response[..Math.Min(500, response.Length)]}");
                return JsonSerializer.Deserialize<NsiResponse>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attempt {attempt}/{RetryCount} failed: {ex.Message}");
            }
        }
        return null;
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
    private class NsiResponse
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("list")]
        public List<List<NsiColumn>> List { get; set; } = [];
    }

    private class NsiColumn
    {
        [JsonPropertyName("column")]
        public string Column { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}




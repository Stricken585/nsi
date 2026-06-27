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
    private readonly string _baseUrl;

    private const int RetryCount = 5;

    public NsiApiService(HttpClient http, ISession db, IConfiguration config)
    {
        _http = http;
        _db = db;
        _userKey = config["Api:UserKey"]!;
        _baseUrl = config["Api:BaseUrl"]!;
    }

    public async Task<int> FetchAndSaveAsync<T>(string identifier) where T : INsiEntity, new()
    {
        int page = 1;
        const int pageSize = 100;
        int savedCount = 0;

        var apiResponse = await FetchPageAsync(identifier, page, pageSize);
        while (apiResponse != null)
        {
            using var tx = _db.BeginTransaction();
            foreach (var row in apiResponse.List)
            {
                var dict = row.ToDictionary(x => x.Column, x => x.Value);
                var entity = new T();
                entity.Fill(dict);
                _db.Save(entity);
                savedCount++;
            }
            await tx.CommitAsync();

            if (page * pageSize >= apiResponse.Total)
                break;

            page++;
            apiResponse = await FetchPageAsync(identifier, page, pageSize);
        }

        return savedCount;
    }

    private async Task<NsiResponse?> FetchPageAsync(string identifier, int page, int pageSize)
    {
        var url = $"{_baseUrl}?identifier={identifier}&userKey={_userKey}&page={page}&size={pageSize}";
        for (int attempt = 1; attempt <= RetryCount; attempt++)
        {
            try
            {
                var response = await _http.GetStringAsync(url);
                return JsonSerializer.Deserialize<NsiResponse>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Attempt {attempt}/{RetryCount} failed: {ex.Message}");
            }
        }
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

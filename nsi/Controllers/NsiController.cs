using Microsoft.AspNetCore.Mvc;
using nsi.Models;
using nsi.Services;
using ISession = NHibernate.ISession;

namespace nsi.Controllers;

public class NsiController : Controller
{
    private readonly NsiApiService _nsi;
    private readonly ISession _db;
    private readonly string _identifier;
    
    public NsiController(NsiApiService nsi, ISession db, IConfiguration config) 
    {
        _nsi = nsi;
        _db = db;
        _identifier = config["Api:Identifier"]!;
    }

    public IActionResult Index()
    {
        var items = _db.Query<MedicalOrganization>().OrderBy(o => o.Code).ToList();
        return View(items);
    }

    public async Task<IActionResult> Import()
    {
        var count = await _nsi.FetchAndSaveAsync<MedicalOrganization>(_identifier);
        return Json(new { message = $"Импортировано записей: {count}" });
    }

    public async Task<IActionResult> Clear()
    {
        using var tx = _db.BeginTransaction();
        await _db.CreateQuery("delete from MedicalOrganization").ExecuteUpdateAsync();
        await tx.CommitAsync();
        return Json(new { message = "База очищена" });
    }
    
}

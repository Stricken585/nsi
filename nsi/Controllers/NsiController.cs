using Microsoft.AspNetCore.Mvc;
using nsi.Models;
using nsi.Services;
using ISession = NHibernate.ISession;

namespace nsi.Controllers;

public class NsiController : Controller
{
    private readonly NsiApiService _nsi;
    private readonly ISession _db;
    private readonly string _medOrgId;
    private readonly string _medProdId;
    
    
    public NsiController(NsiApiService nsi, ISession db, IConfiguration config) 
    {
        _nsi = nsi;
        _db = db;
        _medOrgId = config["Api:MedOrgID"]!;
        _medProdId = config["Api:MedProdID"]!;
    }

    public IActionResult Index(string dict = "MedOrg")
    {
        return dict switch
        {
            "MedOrg" => View("MedOrgIndex", _db.Query<MedicalOrganization>().OrderBy(o => o.Code).ToList()),
            "MedProd" => View("MedProdIndex", _db.Query<MedicalProducts>().OrderBy(o => o.Code).ToList()),
            _ => NotFound()
        };
    }

    public async Task<IActionResult> Import(string dict = "MedOrg")
    {
        var count = dict switch
        {
            "MedOrg" => await _nsi.FetchAndSaveAsync<MedicalOrganization>(_medOrgId),
            "MedProd" => await _nsi.FetchAndSaveAsync<MedicalProducts>(_medProdId),
            _ => throw new ArgumentException($"Неизвестный справочник: {dict}")
        };
        return Json(new { message = $"Импортировано записей: {count} из справочника {dict}"});
    }

    public async Task<IActionResult> Clear(string dict = "MedOrg")
    {
        using var tx = _db.BeginTransaction();
        await (dict switch
        {
            "MedOrg" => _db.CreateQuery("delete from MedicalOrganization").ExecuteUpdateAsync(),
            "MedProd" => _db.CreateQuery("delete from medicalproducts").ExecuteUpdateAsync(),
            _ => throw new ArgumentException($"Неизвестный справочник: {dict}")
        });
        await tx.CommitAsync();
        return Json(new { message = "Таблица очищена" });
    }
    
}

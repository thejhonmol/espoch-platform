using System.Security.Claims;
using ESPOCH.Core.DTOs;
using ESPOCH.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESPOCH.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AusenciasController : ControllerBase
{
    private readonly IAusenciaService _ausenciaService;

    public AusenciasController(IAusenciaService ausenciaService)
    {
        _ausenciaService = ausenciaService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAusenciaDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var resultado = await _ausenciaService.CrearAsync(userId.Value, dto);
        
        if (!resultado.Success)
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }

    [HttpGet("mis")]
    public async Task<IActionResult> GetMisSolicitudes()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var solicitudes = await _ausenciaService.GetMisSolicitudesAsync(userId.Value);
        return Ok(solicitudes);
    }

    [HttpGet("pendientes")]
    [Authorize(Roles = "Admin,JefeDirecto")]
    public async Task<IActionResult> GetPendientesAprobacion()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var solicitudes = await _ausenciaService.GetPendientesAprobacionAsync(userId.Value);
        return Ok(solicitudes);
    }

    [HttpPut("{id}/aprobar")]
    [Authorize(Roles = "Admin,JefeDirecto")]
    public async Task<IActionResult> Aprobar(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var resultado = await _ausenciaService.AprobarAsync(id, userId.Value);
        
        if (!resultado.Success)
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }

    [HttpPut("{id}/rechazar")]
    [Authorize(Roles = "Admin,JefeDirecto")]
    public async Task<IActionResult> Rechazar(int id, [FromBody] AprobarRechazarAusenciaDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var resultado = await _ausenciaService.RechazarAsync(id, userId.Value, dto.MotivoRechazo ?? "");
        
        if (!resultado.Success)
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var solicitudes = await _ausenciaService.GetAllAsync();
        return Ok(solicitudes);
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return null;
        }

        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}

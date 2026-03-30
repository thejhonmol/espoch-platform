using System.Security.Claims;
using ESPOCH.Core.DTOs;
using ESPOCH.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESPOCH.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsistenciaController : ControllerBase
{
    private readonly IAsistenciaService _asistenciaService;

    public AsistenciaController(IAsistenciaService asistenciaService)
    {
        _asistenciaService = asistenciaService;
    }

    [HttpPost("marcar")]
    public async Task<IActionResult> Marcar([FromBody] MarcarEntradaDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var marcacionAbierta = await _asistenciaService.MarcarEntradaAsync(userId.Value, dto);
        
        if (!marcacionAbierta.Success)
        {
            return BadRequest(marcacionAbierta);
        }

        return Ok(marcacionAbierta);
    }

    [HttpPost("marcar/{id}/salida")]
    public async Task<IActionResult> MarcarSalida(int id, [FromBody] MarcarSalidaDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var resultado = await _asistenciaService.MarcarSalidaAsync(userId.Value, id, dto);
        
        if (!resultado.Success)
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }

    [HttpGet("historial")]
    public async Task<IActionResult> GetHistorial()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var historial = await _asistenciaService.GetHistorialAsync(userId.Value);
        return Ok(historial);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var historial = await _asistenciaService.GetAllAsync();
        return Ok(historial);
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

using System.Security.Claims;
using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESPOCH.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUbicacionRepository _ubicacionRepository;
    private readonly IHorarioRepository _horarioRepository;

    public AdminController(
        IUsuarioRepository usuarioRepository,
        IUbicacionRepository ubicacionRepository,
        IHorarioRepository horarioRepository)
    {
        _usuarioRepository = usuarioRepository;
        _ubicacionRepository = ubicacionRepository;
        _horarioRepository = horarioRepository;
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return Ok(usuarios.Select(u => new
        {
            u.IdUsuario,
            u.NombreCompleto,
            u.CorreoInstitucional,
            u.Estado,
            Rol = u.IdRolNavigation.NombreRol,
            u.IdJefeDirecto,
            NombreJefe = u.IdJefeDirectoNavigation?.NombreCompleto
        }));
    }

    [HttpPost("usuarios")]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDto dto)
    {
        var usuario = new Usuario
        {
            NombreCompleto = dto.NombreCompleto,
            CorreoInstitucional = dto.CorreoInstitucional,
            IdRol = dto.IdRol,
            IdJefeDirecto = dto.IdJefeDirecto,
            IdHorario = dto.IdHorario,
            Estado = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _usuarioRepository.AddAsync(usuario);
        return Ok(new { message = "Usuario creado correctamente", usuario });
    }

    [HttpPut("usuarios/{id}")]
    public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] ActualizarUsuarioDto dto)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        usuario.NombreCompleto = dto.NombreCompleto;
        usuario.IdRol = dto.IdRol;
        usuario.IdJefeDirecto = dto.IdJefeDirecto;
        usuario.IdHorario = dto.IdHorario;
        usuario.Estado = dto.Estado;

        await _usuarioRepository.UpdateAsync(usuario);
        return Ok(new { message = "Usuario actualizado correctamente" });
    }

    [HttpGet("ubicaciones")]
    public async Task<IActionResult> GetUbicaciones()
    {
        var ubicaciones = await _ubicacionRepository.GetAllAsync();
        return Ok(ubicaciones);
    }

    [HttpPost("ubicaciones")]
    public async Task<IActionResult> CrearUbicacion([FromBody] Ubicacion ubicacion)
    {
        await _ubicacionRepository.AddAsync(ubicacion);
        return Ok(new { message = "Ubicación creada correctamente", ubicacion });
    }

    [HttpPut("ubicaciones/{id}")]
    public async Task<IActionResult> ActualizarUbicacion(int id, [FromBody] Ubicacion ubicacion)
    {
        var existente = await _ubicacionRepository.GetByIdAsync(id);
        if (existente == null)
        {
            return NotFound();
        }

        existente.CodigoUbicacion = ubicacion.CodigoUbicacion;
        existente.Direccion = ubicacion.Direccion;
        existente.Latitud = ubicacion.Latitud;
        existente.Longitud = ubicacion.Longitud;
        existente.RadioPermitidoSede = ubicacion.RadioPermitidoSede;
        existente.Estado = ubicacion.Estado;

        await _ubicacionRepository.UpdateAsync(existente);
        return Ok(new { message = "Ubicación actualizada correctamente" });
    }

    [HttpDelete("ubicaciones/{id}")]
    public async Task<IActionResult> EliminarUbicacion(int id)
    {
        await _ubicacionRepository.DeleteAsync(id);
        return Ok(new { message = "Ubicación eliminada correctamente" });
    }

    [HttpGet("horarios")]
    public async Task<IActionResult> GetHorarios()
    {
        var horarios = await _horarioRepository.GetAllAsync();
        return Ok(horarios);
    }

    [HttpPost("horarios")]
    public async Task<IActionResult> CrearHorario([FromBody] Horario horario)
    {
        await _horarioRepository.AddAsync(horario);
        return Ok(new { message = "Horario creado correctamente", horario });
    }

    [HttpPut("horarios/{id}")]
    public async Task<IActionResult> ActualizarHorario(int id, [FromBody] Horario horario)
    {
        var existente = await _horarioRepository.GetByIdAsync(id);
        if (existente == null)
        {
            return NotFound();
        }

        existente.NombreHorario = horario.NombreHorario;
        existente.HorarioInicio = horario.HorarioInicio;
        existente.HorarioFin = horario.HorarioFin;
        existente.Estado = horario.Estado;

        await _horarioRepository.UpdateAsync(existente);
        return Ok(new { message = "Horario actualizado correctamente" });
    }

    [HttpDelete("horarios/{id}")]
    public async Task<IActionResult> EliminarHorario(int id)
    {
        await _horarioRepository.DeleteAsync(id);
        return Ok(new { message = "Horario eliminado correctamente" });
    }
}

public class CrearUsuarioDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoInstitucional { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public int? IdJefeDirecto { get; set; }
    public int? IdHorario { get; set; }
}

public class ActualizarUsuarioDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public int? IdJefeDirecto { get; set; }
    public int? IdHorario { get; set; }
    public bool Estado { get; set; }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ESPOCH.Core.DTOs;
using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ESPOCH.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IConfiguration _configuration;

    public AuthController(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Iniciar sesión con credenciales (email y contraseña)
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email y contraseña son requeridos" });
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            
            if (usuario == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            // Verificar estado del usuario
            if (!usuario.Estado)
            {
                return Unauthorized(new { message = "Usuario inactivo" });
            }

            // Verificar contraseña (en producción usar BCrypt o similar)
            // Por ahora comparamos directamente - cambiar en producción
            if (usuario.ContrasenaHash != request.Password)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            // Cargar el rol si no está cargado
            if (usuario.IdRolNavigation == null)
            {
                usuario.IdRolNavigation = await _rolRepository.GetByIdAsync(usuario.IdRol);
            }

            var jwtToken = GenerateJwtToken(usuario);
            var expiration = DateTime.UtcNow.AddHours(8);

            return Ok(new TokenResponseDto
            {
                Token = jwtToken,
                Expiration = expiration,
                Usuario = new UsuarioDto
                {
                    IdUsuario = usuario.IdUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    CorreoInstitucional = usuario.CorreoInstitucional,
                    Rol = usuario.IdRolNavigation?.NombreRol ?? "SinRol",
                    IdRol = usuario.IdRol
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error en autenticación: {ex.Message}" });
        }
    }

    /// <summary>
    /// Registrar un nuevo usuario (solo Admin puede crear usuarios)
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var existingUser = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            var rol = await _rolRepository.GetByIdAsync(request.IdRol);
            if (rol == null)
            {
                return BadRequest(new { message = "Rol inválido" });
            }

            var usuario = new Usuario
            {
                NombreCompleto = request.NombreCompleto,
                CorreoInstitucional = request.Email,
                ContrasenaHash = request.Password, // En producción usar BCrypt
                IdRol = request.IdRol,
                IdJefeDirecto = request.IdJefeDirecto,
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            await _usuarioRepository.AddAsync(usuario);

            return Ok(new { message = "Usuario creado exitosamente", usuarioId = usuario.IdUsuario });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al crear usuario: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtener usuario actual
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var usuario = await _usuarioRepository.GetByIdAsync(userId);
        
        if (usuario == null)
        {
            return NotFound();
        }

        return Ok(new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            CorreoInstitucional = usuario.CorreoInstitucional,
            Rol = usuario.IdRolNavigation?.NombreRol ?? "SinRol",
            IdRol = usuario.IdRol
        });
    }

    /// <summary>
    /// Cambiar contraseña
    /// </summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var usuario = await _usuarioRepository.GetByIdAsync(userId);
        
        if (usuario == null)
        {
            return NotFound();
        }

        // Verificar contraseña actual
        if (usuario.ContrasenaHash != request.CurrentPassword)
        {
            return BadRequest(new { message = "Contraseña actual incorrecta" });
        }

        usuario.ContrasenaHash = request.NewPassword;
        await _usuarioRepository.UpdateAsync(usuario);

        return Ok(new { message = "Contraseña cambiada exitosamente" });
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, usuario.CorreoInstitucional),
            new Claim(ClaimTypes.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Role, usuario.IdRolNavigation?.NombreRol ?? "Colaborador")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

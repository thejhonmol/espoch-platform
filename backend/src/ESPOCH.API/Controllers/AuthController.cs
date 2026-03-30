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

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] TokenRequestDto request)
    {
        try
        {
            var httpClient = new HttpClient();
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            var tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "code", request.Code },
                { "redirect_uri", request.RedirectUri }
            };

            var tokenResponse = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));
            
            if (!tokenResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "Error obtaining token from Azure AD" });
            }

            var tokenContent = await tokenResponse.Content.ReadFromJsonAsync<AzureAdTokenResponse>();
            
            if (tokenContent == null)
            {
                return BadRequest(new { message = "Invalid token response" });
            }

            var handler = new JwtSecurityTokenHandler();
            var idToken = handler.ReadJwtToken(tokenContent.Id_token);
            
            var email = idToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = idToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var azureOid = idToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email not found in token" });
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            
            if (usuario == null)
            {
                var rolColaborador = await _rolRepository.GetByIdAsync(3);
                
                usuario = new Usuario
                {
                    NombreCompleto = name ?? email,
                    CorreoInstitucional = email,
                    azureOid = azureOid,
                    IdRol = rolColaborador?.IdRol ?? 3,
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow
                };
                
                await _usuarioRepository.AddAsync(usuario);
            }
            else
            {
                usuario.azureOid = azureOid;
                await _usuarioRepository.UpdateAsync(usuario);
            }

            usuario = await _usuarioRepository.GetByIdAsync(usuario.IdUsuario);

            var jwtToken = GenerateJwtToken(usuario!);
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
                    Rol = usuario.IdRolNavigation.NombreRol,
                    IdRol = usuario.IdRol,
                    AzureOid = usuario.azureOid
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error processing authentication: {ex.Message}" });
        }
    }

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
            Rol = usuario.IdRolNavigation.NombreRol,
            IdRol = usuario.IdRol,
            AzureOid = usuario.azureOid
        });
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
            new Claim(ClaimTypes.Role, usuario.IdRolNavigation.NombreRol)
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

    private class AzureAdTokenResponse
    {
        public string Access_Token { get; set; } = string.Empty;
        public string Id_Token { get; set; } = string.Empty;
    }
}

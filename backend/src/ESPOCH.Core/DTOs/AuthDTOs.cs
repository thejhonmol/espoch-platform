namespace ESPOCH.Core.DTOs;

public class TokenRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
}

public class UsuarioDto
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoInstitucional { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public string? AzureOid { get; set; }
}

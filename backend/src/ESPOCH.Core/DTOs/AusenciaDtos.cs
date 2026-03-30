namespace ESPOCH.Core.DTOs;

public class CrearAusenciaDto
{
    public DateTime FechaAusencia { get; set; }
    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string TipoAusencia { get; set; } = string.Empty;
}

public class AprobarRechazarAusenciaDto
{
    public string? MotivoRechazo { get; set; }
}

public class AusenciaDto
{
    public int IdAusencia { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaAusencia { get; set; }
    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }
    public decimal TotalHoras { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string TipoAusencia { get; set; } = string.Empty;
    public string EstadoAprobacion { get; set; } = string.Empty;
    public int? IdAprobador { get; set; }
    public string? NombreAprobador { get; set; }
    public string? MotivoRechazo { get; set; }
    public DateTime? FechaAprobacion { get; set; }
}

public class AusenciaResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public AusenciaDto? Ausencia { get; set; }
}

namespace ESPOCH.Core.DTOs;

public class MarcarEntradaDto
{
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public int IdUbicacion { get; set; }
    public string Modalidad { get; set; } = "Presencial";
}

public class MarcarSalidaDto
{
    public double Latitud { get; set; }
    public double Longitud { get; set; }
}

public class AsistenciaDto
{
    public int IdAsistencia { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public DateTime? FechaHoraIngreso { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
    public string Modalidad { get; set; } = string.Empty;
    public int IdUbicacion { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public string EstadoPuntualidad { get; set; } = string.Empty;
}

public class MarcarResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public AsistenciaDto? Asistencia { get; set; }
}

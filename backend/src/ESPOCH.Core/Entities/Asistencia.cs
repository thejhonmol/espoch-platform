namespace ESPOCH.Core.Entities;

public class Asistencia
{
    public int IdAsistencia { get; set; }
    public int IdUsuario { get; set; }
    public DateTime? FechaHoraIngreso { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
    public string Modalidad { get; set; } = "Presencial";
    public int IdUbicacion { get; set; }
    public double LatIngreso { get; set; }
    public double LonIngreso { get; set; }
    public double? LatSalida { get; set; }
    public double? LonSalida { get; set; }
    public string EstadoPuntualidad { get; set; } = "Pendiente";
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public Usuario IdUsuarioNavigation { get; set; } = null!;
    public Ubicacion IdUbicacionNavigation { get; set; } = null!;
}

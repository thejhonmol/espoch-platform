namespace ESPOCH.Core.Entities;

public class Ausencia
{
    public int IdAusencia { get; set; }
    public int IdUsuario { get; set; }
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime FechaAusencia { get; set; }
    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }
    public decimal TotalHoras { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string TipoAusencia { get; set; } = string.Empty;
    public string EstadoAprobacion { get; set; } = "Pendiente";
    public int? IdAprobador { get; set; }
    public string? MotivoRechazo { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    
    public Usuario IdUsuarioNavigation { get; set; } = null!;
    public Usuario? IdAprobadorNavigation { get; set; }
}

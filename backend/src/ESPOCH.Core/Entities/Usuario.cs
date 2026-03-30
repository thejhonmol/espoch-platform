namespace ESPOCH.Core.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string CorreoInstitucional { get; set; } = string.Empty;
    public string? azureOid { get; set; }
    public int IdRol { get; set; }
    public int? IdJefeDirecto { get; set; }
    public int? IdHorario { get; set; }
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public Rol IdRolNavigation { get; set; } = null!;
    public Usuario? IdJefeDirectoNavigation { get; set; }
    public Horario? IdHorarioNavigation { get; set; }
    
    public ICollection<Usuario> Colaboradores { get; set; } = new List<Usuario>();
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    public ICollection<Ausencia> AusenciasSolicitadas { get; set; } = new List<Ausencia>();
    public ICollection<Ausencia> AusenciasAprobadas { get; set; } = new List<Ausencia>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESPOCH.Core.Entities;

[Table("Usuarios")]
public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdUsuario { get; set; }

    [Required]
    [MaxLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string CorreoInstitucional { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ContrasenaHash { get; set; }

    [Required]
    public int IdRol { get; set; }

    public int? IdJefeDirecto { get; set; }

    public int? IdHorario { get; set; }

    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("IdRol")]
    public virtual Rol? IdRolNavigation { get; set; }

    [ForeignKey("IdJefeDirecto")]
    public virtual Usuario? IdJefeDirectoNavigation { get; set; }

    [ForeignKey("IdHorario")]
    public virtual Horario? IdHorarioNavigation { get; set; }

    public virtual ICollection<Usuario> Colaboradores { get; set; } = new List<Usuario>();
    public virtual ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    public virtual ICollection<Ausencia> AusenciasSolicitadas { get; set; } = new List<Ausencia>();
    public virtual ICollection<Ausencia> AusenciasAprobadas { get; set; } = new List<Ausencia>();
}

namespace ESPOCH.Core.Entities;

public class Horario
{
    public int IdHorario { get; set; }
    public string NombreHorario { get; set; } = string.Empty;
    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }
    public bool Estado { get; set; } = true;
    
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

namespace ESPOCH.Core.Entities;

public class Ubicacion
{
    public int IdUbicacion { get; set; }
    public string CodigoUbicacion { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public int RadioPermitidoSede { get; set; } = 100;
    public bool Estado { get; set; } = true;
    
    public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
}

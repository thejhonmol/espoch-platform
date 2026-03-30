namespace ESPOCH.Core.Entities;

public class Rol
{
    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
    
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

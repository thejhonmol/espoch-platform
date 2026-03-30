using ESPOCH.Core.Entities;

namespace ESPOCH.Core.Interfaces;

public interface IAsistenciaRepository
{
    Task<IEnumerable<Asistencia>> GetAllAsync();
    Task<Asistencia?> GetByIdAsync(int id);
    Task<IEnumerable<Asistencia>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<Asistencia>> GetByFechaAsync(DateTime fecha);
    Task<Asistencia?> GetMarcacionAbiertaAsync(int idUsuario, DateTime fecha);
    Task<Asistencia> AddAsync(Asistencia asistencia);
    Task UpdateAsync(Asistencia asistencia);
    Task DeleteAsync(int id);
}

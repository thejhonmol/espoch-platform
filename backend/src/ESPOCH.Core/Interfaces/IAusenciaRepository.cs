using ESPOCH.Core.Entities;

namespace ESPOCH.Core.Interfaces;

public interface IAusenciaRepository
{
    Task<IEnumerable<Ausencia>> GetAllAsync();
    Task<Ausencia?> GetByIdAsync(int id);
    Task<IEnumerable<Ausencia>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<Ausencia>> GetPendientesAprobacionAsync(int idAprobador);
    Task<IEnumerable<Ausencia>> GetByFechaAsync(int idUsuario, DateTime fecha);
    Task<decimal> GetTotalHorasByFechaAsync(int idUsuario, DateTime fecha);
    Task<Ausencia> AddAsync(Ausencia ausencia);
    Task UpdateAsync(Ausencia ausencia);
    Task DeleteAsync(int id);
}

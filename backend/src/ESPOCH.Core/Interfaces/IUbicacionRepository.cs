using ESPOCH.Core.Entities;

namespace ESPOCH.Core.Interfaces;

public interface IUbicacionRepository
{
    Task<IEnumerable<Ubicacion>> GetAllAsync();
    Task<Ubicacion?> GetByIdAsync(int id);
    Task<Ubicacion?> GetByCodigoAsync(string codigoUbicacion);
    Task<Ubicacion> AddAsync(Ubicacion ubicacion);
    Task UpdateAsync(Ubicacion ubicacion);
    Task DeleteAsync(int id);
}

using ESPOCH.Core.Entities;

namespace ESPOCH.Core.Interfaces;

public interface IRolRepository
{
    Task<IEnumerable<Rol>> GetAllAsync();
    Task<Rol?> GetByIdAsync(int id);
    Task<Rol> AddAsync(Rol rol);
    Task UpdateAsync(Rol rol);
    Task DeleteAsync(int id);
}

using ESPOCH.Core.Entities;

namespace ESPOCH.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string correoInstitucional);
    Task<IEnumerable<Usuario>> GetByJefeDirectoAsync(int idJefe);
    Task<Usuario> AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DeleteAsync(int id);
}

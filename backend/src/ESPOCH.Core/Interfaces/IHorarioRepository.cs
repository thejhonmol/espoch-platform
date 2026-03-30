using ESPOCH.Core.Entities;

namespace ESPOCH.Core.Interfaces;

public interface IHorarioRepository
{
    Task<IEnumerable<Horario>> GetAllAsync();
    Task<Horario?> GetByIdAsync(int id);
    Task<Horario> AddAsync(Horario horario);
    Task UpdateAsync(Horario horario);
    Task DeleteAsync(int id);
}

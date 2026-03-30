using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using ESPOCH.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESPOCH.Infrastructure.Repositories;

public class HorarioRepository : IHorarioRepository
{
    private readonly ESPOCHDbContext _context;

    public HorarioRepository(ESPOCHDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Horario>> GetAllAsync()
    {
        return await _context.Horarios.Where(h => h.Estado).ToListAsync();
    }

    public async Task<Horario?> GetByIdAsync(int id)
    {
        return await _context.Horarios.FindAsync(id);
    }

    public async Task<Horario> AddAsync(Horario horario)
    {
        _context.Horarios.Add(horario);
        await _context.SaveChangesAsync();
        return horario;
    }

    public async Task UpdateAsync(Horario horario)
    {
        _context.Entry(horario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var horario = await _context.Horarios.FindAsync(id);
        if (horario != null)
        {
            horario.Estado = false;
            await _context.SaveChangesAsync();
        }
    }
}

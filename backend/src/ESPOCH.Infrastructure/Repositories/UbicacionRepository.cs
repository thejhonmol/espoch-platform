using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using ESPOCH.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESPOCH.Infrastructure.Repositories;

public class UbicacionRepository : IUbicacionRepository
{
    private readonly ESPOCHDbContext _context;

    public UbicacionRepository(ESPOCHDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ubicacion>> GetAllAsync()
    {
        return await _context.Ubicaciones.Where(u => u.Estado).ToListAsync();
    }

    public async Task<Ubicacion?> GetByIdAsync(int id)
    {
        return await _context.Ubicaciones.FindAsync(id);
    }

    public async Task<Ubicacion?> GetByCodigoAsync(string codigoUbicacion)
    {
        return await _context.Ubicaciones
            .FirstOrDefaultAsync(u => u.CodigoUbicacion == codigoUbicacion && u.Estado);
    }

    public async Task<Ubicacion> AddAsync(Ubicacion ubicacion)
    {
        _context.Ubicaciones.Add(ubicacion);
        await _context.SaveChangesAsync();
        return ubicacion;
    }

    public async Task UpdateAsync(Ubicacion ubicacion)
    {
        _context.Entry(ubicacion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ubicacion = await _context.Ubicaciones.FindAsync(id);
        if (ubicacion != null)
        {
            ubicacion.Estado = false;
            await _context.SaveChangesAsync();
        }
    }
}

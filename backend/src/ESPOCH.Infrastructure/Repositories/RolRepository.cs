using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using ESPOCH.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESPOCH.Infrastructure.Repositories;

public class RolRepository : IRolRepository
{
    private readonly ESPOCHDbContext _context;

    public RolRepository(ESPOCHDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rol>> GetAllAsync()
    {
        return await _context.Roles.Where(r => r.Estado).ToListAsync();
    }

    public async Task<Rol?> GetByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Rol> AddAsync(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();
        return rol;
    }

    public async Task UpdateAsync(Rol rol)
    {
        _context.Entry(rol).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol != null)
        {
            rol.Estado = false;
            await _context.SaveChangesAsync();
        }
    }
}

using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using ESPOCH.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESPOCH.Infrastructure.Repositories;

public class AusenciaRepository : IAusenciaRepository
{
    private readonly ESPOCHDbContext _context;

    public AusenciaRepository(ESPOCHDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ausencia>> GetAllAsync()
    {
        return await _context.Ausencias
            .Include(a => a.IdUsuarioNavigation)
            .Include(a => a.IdAprobadorNavigation)
            .OrderByDescending(a => a.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<Ausencia?> GetByIdAsync(int id)
    {
        return await _context.Ausencias
            .Include(a => a.IdUsuarioNavigation)
            .Include(a => a.IdAprobadorNavigation)
            .FirstOrDefaultAsync(a => a.IdAusencia == id);
    }

    public async Task<IEnumerable<Ausencia>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.Ausencias
            .Include(a => a.IdAprobadorNavigation)
            .Where(a => a.IdUsuario == idUsuario)
            .OrderByDescending(a => a.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ausencia>> GetPendientesAprobacionAsync(int idAprobador)
    {
        return await _context.Ausencias
            .Include(a => a.IdUsuarioNavigation)
            .Where(a => a.EstadoAprobacion == "Pendiente")
            .OrderByDescending(a => a.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ausencia>> GetByFechaAsync(int idUsuario, DateTime fecha)
    {
        return await _context.Ausencias
            .Where(a => a.IdUsuario == idUsuario && 
                       a.FechaAusencia.Date == fecha.Date &&
                       a.EstadoAprobacion == "Aprobada")
            .ToListAsync();
    }

    public async Task<decimal> GetTotalHorasByFechaAsync(int idUsuario, DateTime fecha)
    {
        var ausencias = await _context.Ausencias
            .Where(a => a.IdUsuario == idUsuario && 
                       a.FechaAusencia.Date == fecha.Date &&
                       (a.EstadoAprobacion == "Aprobada" || a.EstadoAprobacion == "Pendiente"))
            .ToListAsync();
        
        return ausencias.Sum(a => a.TotalHoras);
    }

    public async Task<Ausencia> AddAsync(Ausencia ausencia)
    {
        _context.Ausencias.Add(ausencia);
        await _context.SaveChangesAsync();
        return ausencia;
    }

    public async Task UpdateAsync(Ausencia ausencia)
    {
        _context.Entry(ausencia).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ausencia = await _context.Ausencias.FindAsync(id);
        if (ausencia != null)
        {
            _context.Ausencias.Remove(ausencia);
            await _context.SaveChangesAsync();
        }
    }
}

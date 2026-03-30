using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using ESPOCH.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESPOCH.Infrastructure.Repositories;

public class AsistenciaRepository : IAsistenciaRepository
{
    private readonly ESPOCHDbContext _context;

    public AsistenciaRepository(ESPOCHDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Asistencia>> GetAllAsync()
    {
        return await _context.Asistencias
            .Include(a => a.IdUsuarioNavigation)
            .Include(a => a.IdUbicacionNavigation)
            .OrderByDescending(a => a.FechaHoraIngreso)
            .ToListAsync();
    }

    public async Task<Asistencia?> GetByIdAsync(int id)
    {
        return await _context.Asistencias
            .Include(a => a.IdUsuarioNavigation)
            .Include(a => a.IdUbicacionNavigation)
            .FirstOrDefaultAsync(a => a.IdAsistencia == id);
    }

    public async Task<IEnumerable<Asistencia>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.Asistencias
            .Include(a => a.IdUbicacionNavigation)
            .Where(a => a.IdUsuario == idUsuario)
            .OrderByDescending(a => a.FechaHoraIngreso)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asistencia>> GetByFechaAsync(DateTime fecha)
    {
        return await _context.Asistencias
            .Include(a => a.IdUsuarioNavigation)
            .Include(a => a.IdUbicacionNavigation)
            .Where(a => a.FechaHoraIngreso.HasValue && 
                        a.FechaHoraIngreso.Value.Date == fecha.Date)
            .OrderByDescending(a => a.FechaHoraIngreso)
            .ToListAsync();
    }

    public async Task<Asistencia?> GetMarcacionAbiertaAsync(int idUsuario, DateTime fecha)
    {
        return await _context.Asistencias
            .FirstOrDefaultAsync(a => a.IdUsuario == idUsuario && 
                                     a.FechaHoraIngreso.HasValue &&
                                     a.FechaHoraIngreso.Value.Date == fecha.Date &&
                                     !a.FechaHoraSalida.HasValue);
    }

    public async Task<Asistencia> AddAsync(Asistencia asistencia)
    {
        _context.Asistencias.Add(asistencia);
        await _context.SaveChangesAsync();
        return asistencia;
    }

    public async Task UpdateAsync(Asistencia asistencia)
    {
        _context.Entry(asistencia).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var asistencia = await _context.Asistencias.FindAsync(id);
        if (asistencia != null)
        {
            _context.Asistencias.Remove(asistencia);
            await _context.SaveChangesAsync();
        }
    }
}

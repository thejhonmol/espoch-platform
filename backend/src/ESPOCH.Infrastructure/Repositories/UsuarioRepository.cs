using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;
using ESPOCH.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ESPOCH.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ESPOCHDbContext _context;

    public UsuarioRepository(ESPOCHDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Include(u => u.IdHorarioNavigation)
            .Where(u => u.Estado)
            .ToListAsync();
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Include(u => u.IdHorarioNavigation)
            .Include(u => u.IdJefeDirectoNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
    }

    public async Task<Usuario?> GetByEmailAsync(string correoInstitucional)
    {
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Include(u => u.IdHorarioNavigation)
            .FirstOrDefaultAsync(u => u.CorreoInstitucional == correoInstitucional);
    }

    public async Task<Usuario?> GetByAzureOidAsync(string azureOid)
    {
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Include(u => u.IdHorarioNavigation)
            .FirstOrDefaultAsync(u => u.azureOid == azureOid);
    }

    public async Task<IEnumerable<Usuario>> GetByJefeDirectoAsync(int idJefe)
    {
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Where(u => u.IdJefeDirecto == idJefe && u.Estado)
            .ToListAsync();
    }

    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Entry(usuario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            usuario.Estado = false;
            await _context.SaveChangesAsync();
        }
    }
}

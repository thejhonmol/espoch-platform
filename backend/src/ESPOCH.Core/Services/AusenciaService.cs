using ESPOCH.Core.DTOs;
using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;

namespace ESPOCH.Core.Services;

public interface IAusenciaService
{
    Task<AusenciaResponseDto> CrearAsync(int idUsuario, CrearAusenciaDto dto);
    Task<IEnumerable<AusenciaDto>> GetMisSolicitudesAsync(int idUsuario);
    Task<IEnumerable<AusenciaDto>> GetPendientesAprobacionAsync(int idAprobador);
    Task<AusenciaResponseDto> AprobarAsync(int idAusencia, int idAprobador);
    Task<AusenciaResponseDto> RechazarAsync(int idAusencia, int idAprobador, string motivoRechazo);
    Task<IEnumerable<AusenciaDto>> GetAllAsync();
}

public class AusenciaService : IAusenciaService
{
    private readonly IAusenciaRepository _ausenciaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private const decimal MaximoHorasPorDia = 6m;

    public AusenciaService(
        IAusenciaRepository ausenciaRepository,
        IUsuarioRepository usuarioRepository)
    {
        _ausenciaRepository = ausenciaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AusenciaResponseDto> CrearAsync(int idUsuario, CrearAusenciaDto dto)
    {
        var totalHoras = (decimal)(dto.HorarioFin - dto.HorarioInicio).TotalHours;
        
        var horasExistentes = await _ausenciaRepository.GetTotalHorasByFechaAsync(idUsuario, dto.FechaAusencia.Date);
        var horasAprobadas = horasExistentes;
        
        if (horasExistentes + totalHoras > MaximoHorasPorDia)
        {
            return new AusenciaResponseDto
            {
                Success = false,
                Message = $"Excede el límite máximo de {MaximoHorasPorDia} horas por día. " +
                          $"Ya tiene {horasAprobadas:F1} horas solicitadas para este día."
            };
        }

        var ausencia = new Ausencia
        {
            IdUsuario = idUsuario,
            FechaSolicitud = DateTime.UtcNow,
            FechaAusencia = dto.FechaAusencia,
            HorarioInicio = dto.HorarioInicio,
            HorarioFin = dto.HorarioFin,
            TotalHoras = totalHoras,
            Motivo = dto.Motivo,
            TipoAusencia = dto.TipoAusencia,
            EstadoAprobacion = "Pendiente"
        };

        await _ausenciaRepository.AddAsync(ausencia);
        
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);

        return new AusenciaResponseDto
        {
            Success = true,
            Message = "Solicitud de ausencia creada correctamente",
            Ausencia = MapToDto(ausencia, usuario!)
        };
    }

    public async Task<IEnumerable<AusenciaDto>> GetMisSolicitudesAsync(int idUsuario)
    {
        var ausencias = await _ausenciaRepository.GetByUsuarioAsync(idUsuario);
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        
        var result = new List<AusenciaDto>();
        foreach (var a in ausencias)
        {
            result.Add(await MapToDtoConAprobador(a, usuario!));
        }
        
        return result;
    }

    public async Task<IEnumerable<AusenciaDto>> GetPendientesAprobacionAsync(int idAprobador)
    {
        var aprobador = await _usuarioRepository.GetByIdAsync(idAprobador);
        if (aprobador == null)
        {
            return new List<AusenciaDto>();
        }

        IEnumerable<Ausencia> ausencias;
        
        if (aprobador.IdRol == 1)
        {
            ausencias = await _ausenciaRepository.GetAllAsync();
            ausencias = ausencias.Where(a => a.EstadoAprobacion == "Pendiente");
        }
        else
        {
            var colaboradores = await _usuarioRepository.GetByJefeDirectoAsync(idAprobador);
            var idsColaboradores = colaboradores.Select(c => c.IdUsuario);
            ausencias = await _ausenciaRepository.GetAllAsync();
            ausencias = ausencias.Where(a => idsColaboradores.Contains(a.IdUsuario) && a.EstadoAprobacion == "Pendiente");
        }

        var result = new List<AusenciaDto>();
        foreach (var a in ausencias)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(a.IdUsuario);
            result.Add(await MapToDtoConAprobador(a, usuario!));
        }
        
        return result;
    }

    public async Task<AusenciaResponseDto> AprobarAsync(int idAusencia, int idAprobador)
    {
        var ausencia = await _ausenciaRepository.GetByIdAsync(idAusencia);
        if (ausencia == null)
        {
            return new AusenciaResponseDto
            {
                Success = false,
                Message = "Solicitud no encontrada"
            };
        }

        if (ausencia.EstadoAprobacion != "Pendiente")
        {
            return new AusenciaResponseDto
            {
                Success = false,
                Message = "La solicitud ya fue procesada"
            };
        }

        ausencia.EstadoAprobacion = "Aprobada";
        ausencia.IdAprobador = idAprobador;
        ausencia.FechaAprobacion = DateTime.UtcNow;

        await _ausenciaRepository.UpdateAsync(ausencia);
        
        var usuario = await _usuarioRepository.GetByIdAsync(ausencia.IdUsuario);
        var aprobador = await _usuarioRepository.GetByIdAsync(idAprobador);

        return new AusenciaResponseDto
        {
            Success = true,
            Message = "Solicitud aprobada correctamente",
            Ausencia = MapToDto(ausencia, usuario!, aprobador)
        };
    }

    public async Task<AusenciaResponseDto> RechazarAsync(int idAusencia, int idAprobador, string motivoRechazo)
    {
        if (string.IsNullOrWhiteSpace(motivoRechazo))
        {
            return new AusenciaResponseDto
            {
                Success = false,
                Message = "El motivo de rechazo es obligatorio"
            };
        }

        var ausencia = await _ausenciaRepository.GetByIdAsync(idAusencia);
        if (ausencia == null)
        {
            return new AusenciaResponseDto
            {
                Success = false,
                Message = "Solicitud no encontrada"
            };
        }

        if (ausencia.EstadoAprobacion != "Pendiente")
        {
            return new AusenciaResponseDto
            {
                Success = false,
                Message = "La solicitud ya fue procesada"
            };
        }

        ausencia.EstadoAprobacion = "Rechazada";
        ausencia.IdAprobador = idAprobador;
        ausencia.MotivoRechazo = motivoRechazo;
        ausencia.FechaAprobacion = DateTime.UtcNow;

        await _ausenciaRepository.UpdateAsync(ausencia);
        
        var usuario = await _usuarioRepository.GetByIdAsync(ausencia.IdUsuario);
        var aprobador = await _usuarioRepository.GetByIdAsync(idAprobador);

        return new AusenciaResponseDto
        {
            Success = true,
            Message = "Solicitud rechazada correctamente",
            Ausencia = MapToDto(ausencia, usuario!, aprobador)
        };
    }

    public async Task<IEnumerable<AusenciaDto>> GetAllAsync()
    {
        var ausencias = await _ausenciaRepository.GetAllAsync();
        var result = new List<AusenciaDto>();
        
        foreach (var a in ausencias)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(a.IdUsuario);
            result.Add(await MapToDtoConAprobador(a, usuario!));
        }
        
        return result;
    }

    private AusenciaDto MapToDto(Ausencia ausencia, Usuario usuario, Usuario? aprobador)
    {
        return new AusenciaDto
        {
            IdAusencia = ausencia.IdAusencia,
            IdUsuario = ausencia.IdUsuario,
            NombreUsuario = usuario.NombreCompleto,
            FechaSolicitud = ausencia.FechaSolicitud,
            FechaAusencia = ausencia.FechaAusencia,
            HorarioInicio = ausencia.HorarioInicio,
            HorarioFin = ausencia.HorarioFin,
            TotalHoras = ausencia.TotalHoras,
            Motivo = ausencia.Motivo,
            TipoAusencia = ausencia.TipoAusencia,
            EstadoAprobacion = ausencia.EstadoAprobacion,
            IdAprobador = ausencia.IdAprobador,
            NombreAprobador = aprobador?.NombreCompleto,
            MotivoRechazo = ausencia.MotivoRechazo,
            FechaAprobacion = ausencia.FechaAprobacion
        };
    }

    private async Task<AusenciaDto> MapToDtoConAprobador(Ausencia ausencia, Usuario usuario)
    {
        Usuario? aprobador = null;
        if (ausencia.IdAprobador.HasValue)
        {
            aprobador = await _usuarioRepository.GetByIdAsync(ausencia.IdAprobador.Value);
        }
        return MapToDto(ausencia, usuario, aprobador);
    }
}

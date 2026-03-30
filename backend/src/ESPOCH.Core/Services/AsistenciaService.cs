using ESPOCH.Core.DTOs;
using ESPOCH.Core.Entities;
using ESPOCH.Core.Interfaces;

namespace ESPOCH.Core.Services;

public interface IAsistenciaService
{
    Task<MarcarResponseDto> MarcarEntradaAsync(int idUsuario, MarcarEntradaDto dto);
    Task<MarcarResponseDto> MarcarSalidaAsync(int idUsuario, int idAsistencia, MarcarSalidaDto dto);
    Task<IEnumerable<AsistenciaDto>> GetHistorialAsync(int idUsuario);
    Task<IEnumerable<AsistenciaDto>> GetAllAsync();
}

public class AsistenciaService : IAsistenciaService
{
    private readonly IAsistenciaRepository _asistenciaRepository;
    private readonly IUbicacionRepository _ubicacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IGeolocationService _geolocationService;

    public AsistenciaService(
        IAsistenciaRepository asistenciaRepository,
        IUbicacionRepository ubicacionRepository,
        IUsuarioRepository usuarioRepository,
        IGeolocationService geolocationService)
    {
        _asistenciaRepository = asistenciaRepository;
        _ubicacionRepository = ubicacionRepository;
        _usuarioRepository = usuarioRepository;
        _geolocationService = geolocationService;
    }

    public async Task<MarcarResponseDto> MarcarEntradaAsync(int idUsuario, MarcarEntradaDto dto)
    {
        var ubicacion = await _ubicacionRepository.GetByIdAsync(dto.IdUbicacion);
        if (ubicacion == null)
        {
            return new MarcarResponseDto
            {
                Success = false,
                Message = "Ubicación no encontrada"
            };
        }

        if (!_geolocationService.EstaEnRangoPermitido(dto.Latitud, dto.Longitud, ubicacion))
        {
            var distancia = _geolocationService.CalcularDistancia(
                dto.Latitud, dto.Longitud, ubicacion.Latitud, ubicacion.Longitud);
            return new MarcarResponseDto
            {
                Success = false,
                Message = $"Ubicación fuera del rango permitido. Distancia: {distancia:F2} metros (máximo: {ubicacion.RadioPermitidoSede}m)"
            };
        }

        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        var estadoPuntualidad = "Presente";
        
        if (usuario?.IdHorarioNavigation != null)
        {
            var horaActual = DateTime.UtcNow.TimeOfDay;
            if (horaActual > usuario.IdHorarioNavigation.HorarioInicio.Add(TimeSpan.FromMinutes(15)))
            {
                estadoPuntualidad = "Tardanza";
            }
        }

        var asistencia = new Asistencia
        {
            IdUsuario = idUsuario,
            FechaHoraIngreso = DateTime.UtcNow,
            Modalidad = dto.Modalidad,
            IdUbicacion = dto.IdUbicacion,
            LatIngreso = dto.Latitud,
            LonIngreso = dto.Longitud,
            EstadoPuntualidad = estadoPuntualidad
        };

        await _asistenciaRepository.AddAsync(asistencia);

        return new MarcarResponseDto
        {
            Success = true,
            Message = estadoPuntualidad == "Presente" ? "Entrada registrada correctamente" : "Entrada registrada con tardanza",
            Asistencia = MapToDto(asistencia, usuario!, ubicacion)
        };
    }

    public async Task<MarcarResponseDto> MarcarSalidaAsync(int idUsuario, int idAsistencia, MarcarSalidaDto dto)
    {
        var asistencia = await _asistenciaRepository.GetByIdAsync(idAsistencia);
        if (asistencia == null || asistencia.IdUsuario != idUsuario)
        {
            return new MarcarResponseDto
            {
                Success = false,
                Message = "Asistencia no encontrada"
            };
        }

        if (asistencia.FechaHoraSalida.HasValue)
        {
            return new MarcarResponseDto
            {
                Success = false,
                Message = "Ya se ha registrado la salida"
            };
        }

        asistencia.FechaHoraSalida = DateTime.UtcNow;
        asistencia.LatSalida = dto.Latitud;
        asistencia.LonSalida = dto.Longitud;

        await _asistenciaRepository.UpdateAsync(asistencia);

        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        var ubicacion = await _ubicacionRepository.GetByIdAsync(asistencia.IdUbicacion);

        return new MarcarResponseDto
        {
            Success = true,
            Message = "Salida registrada correctamente",
            Asistencia = MapToDto(asistencia, usuario!, ubicacion!)
        };
    }

    public async Task<IEnumerable<AsistenciaDto>> GetHistorialAsync(int idUsuario)
    {
        var asignaciones = await _asistenciaRepository.GetByUsuarioAsync(idUsuario);
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
        
        return await Task.WhenAll(asignaciones.Select(async a =>
        {
            var ubicacion = await _ubicacionRepository.GetByIdAsync(a.IdUbicacion);
            return MapToDto(a, usuario!, ubicacion!);
        }));
    }

    public async Task<IEnumerable<AsistenciaDto>> GetAllAsync()
    {
        var asignaciones = await _asistenciaRepository.GetAllAsync();
        var result = new List<AsistenciaDto>();
        
        foreach (var a in asignaciones)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(a.IdUsuario);
            var ubicacion = await _ubicacionRepository.GetByIdAsync(a.IdUbicacion);
            result.Add(MapToDto(a, usuario!, ubicacion!));
        }
        
        return result;
    }

    private AsistenciaDto MapToDto(Asistencia asistencia, Usuario usuario, Ubicacion ubicacion)
    {
        return new AsistenciaDto
        {
            IdAsistencia = asistencia.IdAsistencia,
            IdUsuario = asistencia.IdUsuario,
            NombreUsuario = usuario.NombreCompleto,
            FechaHoraIngreso = asistencia.FechaHoraIngreso,
            FechaHoraSalida = asistencia.FechaHoraSalida,
            Modalidad = asistencia.Modalidad,
            IdUbicacion = asistencia.IdUbicacion,
            Ubicacion = ubicacion.Direccion,
            EstadoPuntualidad = asistencia.EstadoPuntualidad
        };
    }
}

using ESPOCH.Core.Entities;

namespace ESPOCH.Core.Services;

public interface IGeolocationService
{
    double CalcularDistancia(double lat1, double lon1, double lat2, double lon2);
    bool EstaEnRangoPermitido(double latUsuario, double lonUsuario, Ubicacion ubicacion);
}

public class GeolocationService : IGeolocationService
{
    private const double RadioTierraKm = 6371;
    
    public double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = GradosARadianes(lat2 - lat1);
        var dLon = GradosARadianes(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(GradosARadianes(lat1)) * Math.Cos(GradosARadianes(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distanciaKm = RadioTierraKm * c;
        
        return distanciaKm * 1000;
    }
    
    public bool EstaEnRangoPermitido(double latUsuario, double lonUsuario, Ubicacion ubicacion)
    {
        var distancia = CalcularDistancia(latUsuario, lonUsuario, ubicacion.Latitud, ubicacion.Longitud);
        return distancia <= ubicacion.RadioPermitidoSede;
    }
    
    private static double GradosARadianes(double grados)
    {
        return grados * Math.PI / 180;
    }
}

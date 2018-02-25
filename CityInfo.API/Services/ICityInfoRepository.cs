using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        bool CityExists(int cityId);

        // Either this...
        IEnumerable<City> GetCities();
        // ... or this. If we return this, the consumer can build on the return...adding OrderBy, etc. But also leaks persistence related logic
        //IQueryable<City> GetCities();

        City GetCity(int cityId, bool includePointsOfInterest);

        IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);

        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);
        bool Save();
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
    }
}

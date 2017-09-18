using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using System.Linq;

namespace HousingBuyingHelper
{
    public class RouteCalculator
    {
        public Route GetRoute(string origin, string dest)
        {
            var directionsRequest = new DirectionsRequest()
            {
                ApiKey = "AIzaSyAOzM-wrIj6ANCLEf3XI37mPc_JH0LDx5U",
                Origin = origin,
                Destination = dest,
            };

            DirectionsResponse directions = GoogleMaps.Directions.Query(directionsRequest);

            if (!directions.Routes.Any() || !directions.Routes.First().Legs.Any())
            {
                return null;
            }

            var leg = directions.Routes.First().Legs.First();
            var distanceInMeters = (double)leg.Distance.Value;
            var distanceInMiles = distanceInMeters * 0.000621371192237;
            var durationInMinutes = leg.Duration.Value.TotalMinutes;

            return new Route { DistanceInMiles = distanceInMiles, DurationInMinutes = durationInMinutes };
        }
    }
}

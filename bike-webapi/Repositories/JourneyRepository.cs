using Microsoft.EntityFrameworkCore;
using bike_webapi.Models;
using bike_webapi.Interfaces;
using bike_webapi.Data;
using bike_webapi.Dto;

namespace bike_webapi.Repositories
{
    public class JourneyRepository : IJourneyRepository
    {
        private readonly AppDbContext _context;

        public JourneyRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool AddJourney(Journey journey)
        {
            _context.Add(journey);

            return _context.SaveChanges() > 0;
        }

        public PagedResultDto<Journey> GetJourneys(
            int pageSize,
            int page, 
            string? orderBy, 
            string? search,
            int departureStationId,
            int returnStationId,
            int month)
        {
            IQueryable<Journey> journeys =  _context.Journeys
                .Include(j => j.DepartureStation)
                .Include(j => j.ReturnStation);

            if (month > 0 && month <= 12)
            {
                journeys = journeys.Where(j => j.Departure.Month == month);
            }

            if (departureStationId > 0)
            {
                journeys = journeys.Where(j => j.DepartureStationId == departureStationId);
            }

            if (returnStationId > 0)
            {
                journeys = journeys.Where(j => j.ReturnStationId == returnStationId);
            }

            if (!String.IsNullOrWhiteSpace(search))
            {
                journeys = journeys.Where(j => 
                    j.DepartureStation.Name.Contains(search) ||
                    j.DepartureStation.Namn.Contains(search) ||
                    j.DepartureStation.Nimi.Contains(search) ||
                    j.DepartureStation.Osoite.Contains(search) ||
                    j.DepartureStation.Adress.Contains(search) ||
                    j.DepartureStation.Stad.Contains(search) ||
                    j.DepartureStation.Kaupunki.Contains(search) ||
                    j.ReturnStation.Name.Contains(search) ||
                    j.ReturnStation.Namn.Contains(search) ||
                    j.ReturnStation.Nimi.Contains(search) ||
                    j.ReturnStation.Osoite.Contains(search) ||
                    j.ReturnStation.Adress.Contains(search) ||
                    j.ReturnStation.Stad.Contains(search) ||
                    j.ReturnStation.Kaupunki.Contains(search)
                );
            }
            
            switch(orderBy)
            {
                case "departureAscending":
                    journeys = journeys.OrderBy(j => j.Departure);
                    break;

                case "departureDescending":
                    journeys = journeys.OrderByDescending(j => j.Departure);
                    break;
                
                case "returnAscending":
                    journeys = journeys.OrderBy(j => j.Return);
                    break;

                case "returnDescending":
                    journeys = journeys.OrderByDescending(j => j.Return);
                    break;
                
                case "departureStationAscending":
                    journeys = journeys.OrderBy(j => j.DepartureStation.Nimi).ThenBy(j => j.Departure);
                    break;
                
                case "departureStationDescending":
                    journeys = journeys.OrderByDescending(j => j.DepartureStation.Nimi).ThenBy(j => j.Departure);
                    break;
                
                case "returnStationAscending":
                    journeys = journeys.OrderBy(j => j.ReturnStation.Nimi).ThenBy(j => j.Return);
                    break;
                
                case "returnStationDescending":
                    journeys = journeys.OrderByDescending(j => j.ReturnStation.Nimi).ThenBy(j => j.Return);
                    break;
                
                case "distanceAscending":
                    journeys = journeys.OrderBy(j => j.CoveredDistance).ThenBy(j => j.Departure);
                    break;
                
                case "distanceDescending":
                    journeys = journeys.OrderByDescending(j => j.CoveredDistance).ThenBy(j => j.Departure);
                    break;
                
                case "durationAscending":
                    journeys = journeys.OrderBy(j => j.Duration).ThenBy(j => j.Departure);
                    break;

                case "durationDescending":
                    journeys = journeys.OrderByDescending(j => j.Duration).ThenBy(j => j.Departure);
                    break;
                
                default:
                    journeys = journeys.OrderBy(j => j.Departure);
                    break;
            }

            var total = journeys.Count();
            
            journeys = journeys
                .Skip(pageSize*(page - 1))
                .Take(pageSize);
            
            return new PagedResultDto<Journey>() { Total = total, Result = journeys.ToList() };
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using bike_webapi.Interfaces;
using bike_webapi.Dto;
using bike_webapi.Models;

namespace bike_webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private IStationRepository _stationRepository;
        private IMapper _mapper;

        public StationsController(IStationRepository stationRepository, IMapper mapper)
        {
            _stationRepository = stationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetStations([FromQuery]int pageSize = 10, [FromQuery]int page = 1, [FromQuery]string? search = "")
        {
            var stations = _stationRepository.GetStations(pageSize, page, search);

            var mappedPages = _mapper.Map<PagedResultDto<StationDto>>(stations);

            return Ok(mappedPages);
        }

        [HttpGet("{id}")]
        public IActionResult GetStation(int id)
        {
            var station = _stationRepository.GetStation(id);

            if (station == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StationDto>(station));
        }

        [HttpGet("{id}/details")]
        public IActionResult GetStationDetails(int id, [FromQuery]int month)
        {
            var station = _stationRepository.GetStation(id);

            if (station == null)
            {
                return NotFound();
            }

            var details = _stationRepository.GetDetails(id, month);

            details.Top5Destinations = _mapper.Map<List<CountedResultDto<StationDto>>>(_stationRepository.GetTop5Destinations(id, month));
            details.Top5Origins = _mapper.Map<List<CountedResultDto<StationDto>>>(_stationRepository.GetTop5Origins(id, month));

            return Ok(details);
        }

        [HttpPost]
        public IActionResult CreateStation([FromBody]StationDto stationDto)
        {
            if (stationDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var station = _stationRepository.GetStation(stationDto.Id);

            if (station != null)
            {
                return Conflict("Station ID already exists");
            }

            var newStation = _mapper.Map<Station>(stationDto);

            if (!_stationRepository.AddStation(newStation))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(newStation);


        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EVStation_basedRentalSystem.Services.CarAPI.Models;
using EVStation_basedRentalSystem.Services.CarAPI.Services.IService;
using EVStation_basedRentalSystem.Services.CarAPI.Models.DTO;
using AutoMapper;
using System.Collections.Generic;

namespace EVStation_basedRentalSystem.Services.CarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IStationService _stationService;
        private readonly IMapper _mapper;
        private readonly ILogger<CarController> _logger;


        public CarController(ICarService carService, IStationService stationService, IMapper mapper, ILogger<CarController> logger)
        {
            _carService = carService;
            _stationService = stationService;
            _mapper = mapper;
            _logger = logger;
        }

        // ---------------- GET all cars (raw Car) ----------------
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _carService.GetAllCarsAsync();
            return Ok(cars);
        }

        // ---------------- GET single car by ID ----------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();

            var carDto = _mapper.Map<CarDto>(car);
            carDto.Station = await _stationService.GetStationByIdAsync(car.StationId);

            return Ok(carDto);
        }

        // ---------------- POST create new car ----------------
        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _carService.AddCarAsync(car);
            return CreatedAtAction(nameof(GetCarById), new { id = created.Id }, created);
        }

        // ---------------- PUT update existing car ----------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] Car car)
        {
            if (id != car.Id)
                return BadRequest("Car ID mismatch");

            var updated = await _carService.UpdateCarAsync(car);
            if (updated == null) return NotFound();

            return NoContent();
        }

        // ---------------- DELETE car ----------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var result = await _carService.DeleteCarAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // ---------------- GET cars by Station ----------------
        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetCarsByStation(int stationId)
        {
            var cars = await _carService.GetCarsByStationIdAsync(stationId);
            return Ok(cars);
        }

        // ---------------- GET available cars ----------------
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCars()
        {
            var cars = await _carService.GetAvailableCarsAsync();
            return Ok(cars);
        }

        // ---------------- PATCH update car state ----------------
        [HttpPatch("{id}/state")]
        public async Task<IActionResult> UpdateState(int id, [FromBody] string newState)
        {
            var result = await _carService.UpdateCarStateAsync(id, newState);
            if (!result) return NotFound();
            return NoContent();
        }

        // ---------------- PUT assign car to station ----------------
        [HttpPut("{carId}/assign/{stationId}")]
        public async Task<IActionResult> AssignCarToStation(int carId, int stationId)
        {
            var car = await _carService.GetCarByIdAsync(carId);
            if (car == null) return NotFound("Car not found");

            var station = await _stationService.GetStationByIdAsync(stationId);
            if (station == null) return BadRequest("Station not found");

            car.StationId = stationId;
            await _carService.UpdateCarAsync(car);

            return Ok(new
            {
                message = $"Car {carId} successfully assigned to Station {stationId}",
                carId,
                stationId
            });
        }

        // ---------------- GET all cars with station info (DTO) ----------------
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetAllCarsWithStation()
        {
            var carDtos = await _carService.GetAllCarsWithStationAsync();
            return Ok(carDtos);
        }
        [HttpPost("block")]
        public async Task<IActionResult> BlockCar([FromBody] BlockCarRequest request)
        {
            if (request == null || request.CarId <= 0)
                return BadRequest("Invalid request");

            var blockId = await _carService.BlockCarAsync(request.CarId, request.StartTime, request.EndTime);
            if (blockId == null)
                return BadRequest($"Car {request.CarId} could not be blocked (maybe unavailable or already blocked).");

            _logger.LogInformation("Car {CarId} blocked from {Start} to {End} with blockId {BlockId}",
                request.CarId, request.StartTime, request.EndTime, blockId);

            return Ok(new
            {
                message = "Car blocked successfully",
                request.CarId,
                request.StartTime,
                request.EndTime,
                BlockId = blockId
            });
        }

        // ---------------- POST unblock xe theo blockId ----------------
        [HttpPost("unblock/{blockId}")]
        public async Task<IActionResult> UnblockCar(int blockId)
        {
            var success = await _carService.UnblockCarAsync(blockId);
            if (!success)
                return NotFound($"Block with ID {blockId} not found or already unblocked.");

            _logger.LogInformation("Block {BlockId} unblocked", blockId);
            return Ok(new { message = "Car unblocked successfully", BlockId = blockId });
        }

        // ---------------- GET danh sách block ----------------
        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedCars([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var blocks = await _carService.GetBlockedCarsAsync(from, to);
            return Ok(blocks);
        }
    }

    // Request model
    public class BlockCarRequest
    {
        public int CarId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
}

     


    // Request model
    
}


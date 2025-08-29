using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Services;

namespace MotorcycleRental.Api.Controllers
{
    [ApiController]
    [Route("motos")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly IMotorcycleService _motorcycleService;
        private readonly ILogger<MotorcyclesController> _logger;

        public MotorcyclesController(IMotorcycleService motorcycleService, ILogger<MotorcyclesController> logger)
        {
            _motorcycleService = motorcycleService;
            _logger = logger;
        }

        /// <summary>
        /// Cadastrar uma nova moto
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MotorcycleResponseDto>> CreateMotorcycle([FromBody] CreateMotorcycleDto request)
        {
            try
            {
                var result = await _motorcycleService.CreateAsync(request);
                return CreatedAtAction(nameof(GetMotorcycleById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation: {Message}", ex.Message);
                return BadRequest(new ErrorResponseDto { Message = ex.Message });
            }
        }

        /// <summary>
        /// Consultar motos existentes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MotorcycleResponseDto>>> GetMotorcycles([FromQuery] string? placa = null)
        {
            var motorcycles = await _motorcycleService.GetAllAsync(placa);
            return Ok(motorcycles);
        }

        /// <summary>
        /// Consultar moto por id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MotorcycleResponseDto>> GetMotorcycleById(string id)
        {
            try
            {
                var motorcycle = await _motorcycleService.GetByIdAsync(id);
                return Ok(motorcycle);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Message = "Moto não encontrada" });
            }
        }

        /// <summary>
        /// Modificar a placa de uma moto
        /// </summary>
        [HttpPut("{id}/placa")]
        public async Task<ActionResult<MotorcycleResponseDto>> UpdateLicensePlate(string id, [FromBody] UpdateMotorcycleLicensePlateDto request)
        {
            try
            {
                var result = await _motorcycleService.UpdateLicensePlateAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Message = "Moto não encontrada" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponseDto { Message = ex.Message });
            }
        }

        /// <summary>
        /// Remover uma moto
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotorcycle(string id)
        {
            try
            {
                await _motorcycleService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Message = "Moto não encontrada" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponseDto { Message = ex.Message });
            }
        }
    }
}
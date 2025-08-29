using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Services;


namespace MotorcycleRental.Api.Controllers
{
    [ApiController]
    [Route("locacao")]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly ILogger<RentalsController> _logger;

        public RentalsController(IRentalService rentalService, ILogger<RentalsController> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

        /// <summary>
        /// Alugar uma moto
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RentalResponseDto>> CreateRental([FromBody] CreateRentalDto request)
        {
            try
            {
                var result = await _rentalService.CreateRentalAsync(request);
                return CreatedAtAction(nameof(GetRentalById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponseDto { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponseDto { Message = ex.Message });
            }
        }

        /// <summary>
        /// Consultar locação por id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RentalResponseDto>> GetRentalById(string id)
        {
            try
            {
                var rental = await _rentalService.GetByIdAsync(id);
                return Ok(rental);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Message = "Locação não encontrada" });
            }
        }

        /// <summary>
        /// Informar data de devolução e consultar valor total
        /// </summary>
        [HttpPut("{id}/devolucao")]
        public async Task<IActionResult> ReturnRental(string id, [FromBody] DateTime returnDate)
        {
            try
            {
                var result = await _rentalService.ReturnRentalAsync(id, returnDate);
                return Ok(new { mensagem = "Data de devolução informada com sucesso"});
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Message = "Locação não encontrada" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponseDto { Message = ex.Message });
            }
        }
    }
}
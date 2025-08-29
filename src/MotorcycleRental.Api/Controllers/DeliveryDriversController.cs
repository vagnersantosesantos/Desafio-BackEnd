using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Application.DTOs;
using MotorcycleRental.Application.Interfaces.Services;

namespace MotorcycleRental.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryDriversController : ControllerBase
    {
        private readonly IDeliveryDriverService _deliveryDriverService;
        private readonly ILogger<DeliveryDriversController> _logger;

        public DeliveryDriversController(IDeliveryDriverService deliveryDriverService, ILogger<DeliveryDriversController> logger)
        {
            _deliveryDriverService = deliveryDriverService;
            _logger = logger;
        }

        /// <summary>
        /// Cadastrar entregador
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DeliveryDriverResponseDto>> CreateDeliveryDriver([FromBody] CreateDeliveryDriverDto request)
        {
            try
            {
                var result = await _deliveryDriverService.CreateAsync(request);
                return CreatedAtAction(nameof(GetDeliveryDriverById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation: {Message}", ex.Message);
                return BadRequest(new ErrorResponseDto { Message = ex.Message });
            }
        }

        /// <summary>
        /// Consultar entregador por id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryDriverResponseDto>> GetDeliveryDriverById(string id)
        {
            try
            {
                var driver = await _deliveryDriverService.GetByIdAsync(id);
                return Ok(driver);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Message = "Entregador não encontrado" });
            }
        }

        /// <summary>
        /// Enviar foto da CNH
        /// </summary>
        [HttpPost("{id}/cnh")]
        public async Task<IActionResult> UploadLicenseImage(string id, IFormFile imagem_cnh)
        {
            try
            {
                if (imagem_cnh == null || imagem_cnh.Length == 0)
                    return BadRequest(new ErrorResponseDto { Message = "Imagem da CNH é obrigatória" });

                await _deliveryDriverService.UpdateLicenseImageAsync(id, imagem_cnh);
                return Ok(new { mensagem = "Imagem da CNH enviada com sucesso" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Message = "Entregador não encontrado" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponseDto { Message = ex.Message });
            }
        }
    }
}
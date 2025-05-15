using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using loue_ma_caisse.Services;
using loue_ma_caisse.DTOs;
using loue_ma_caisse.Models;

namespace loue_ma_caisse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles([FromQuery] VehicleFilterDto filter)
    {
        try
        {
            var vehicles = await _vehicleService.GetVehiclesAsync(filter);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicle(string id)
    {
        try
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound(new { message = "Véhicule non trouvé" });

            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Vehicle>> CreateVehicle(CreateVehicleDto vehicleDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var vehicle = await _vehicleService.CreateVehicleAsync(userId, vehicleDto);
            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<Vehicle>> UpdateVehicle(string id, UpdateVehicleDto vehicleDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var vehicle = await _vehicleService.UpdateVehicleAsync(id, userId, vehicleDto);
            if (vehicle == null)
                return NotFound(new { message = "Véhicule non trouvé ou vous n'êtes pas autorisé à le modifier" });

            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _vehicleService.DeleteVehicleAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }    [Authorize]
    [HttpGet("me")]    public async Task<ActionResult<IEnumerable<Vehicle>>> GetMyVehicles()
    {
        try
        {
            // Log the claims for debugging
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"UserId extrait: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UserId non trouvé dans le token");
                return Unauthorized(new { message = "Utilisateur non authentifié" });
            }
                
            var vehicles = await _vehicleService.GetUserVehiclesAsync(userId);
            Console.WriteLine($"Véhicules récupérés avec succès");
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur dans GetMyVehicles: {ex}");
            return BadRequest(new { message = "Une erreur est survenue lors de la récupération des véhicules" });
        }
    }
}
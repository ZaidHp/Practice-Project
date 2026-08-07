using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaTrack.Application.DTOs.Purchase;
using PharmaTrack.Application.Interfaces;

namespace PharmaTrack.API.Controllers;

[Authorize(Roles = "Admin, Store Manager")]
[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequestDto purchaseDto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("User ID claim is missing or invalid.");
        }

        var username = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        var response = await _purchaseService.AddPurchasesAsync(
            purchaseDto,
            userId,
            username);

        if (response.Success)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPurchases()
    {
        var response = await _purchaseService.GetAllPurchasesAsync();

        if (response.Success)
            return Ok(response);

        return BadRequest(response);
    }

}
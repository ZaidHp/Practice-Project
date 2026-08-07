using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaTrack.Application.DTOs.Supplier;
using PharmaTrack.Application.Interfaces;

namespace PharmaTrack.Api.Controllers;

[Authorize(Roles = "Admin, Store Manager")]
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var response = await _supplierService.GetAllSuppliersAsync(page, pageSize, search);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _supplierService.GetSupplierById(id);
        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var username = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        var response = await _supplierService.CreateSupplierAsync(dto, username);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "System";
        var response = await _supplierService.UpdateSupplierAsync(id, dto, userName);

        if (!response.Success)
        {
            if (response.Message == "Supplier not found.")
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var response = await _supplierService.DeleteSupplierAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var response = await _supplierService.GetSuppliersAsync();
        return Ok(response);
    }
}
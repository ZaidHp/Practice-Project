using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaTrack.Application.DTOs.Medicine;
using PharmaTrack.Application.Interfaces;

namespace PharmaTrack.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[Controller]")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;
    
    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var response = await _medicineService.GetAllMedicineAsync(page, pageSize, search);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicineDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var username = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        var response = await _medicineService.CreateMedicineAsync(dto, username);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _medicineService.GetMedicineByIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicineDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var username = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        var response = await _medicineService.UpdateMedicineAsync(id, dto, username);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _medicineService.DeleteMedicineAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllMedicines()
    {
        var response = await _medicineService.GetAllMedicinesAsync();
        return Ok(response);
    }
}
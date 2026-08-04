using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaTrack.Application.DTOs.User;
using PharmaTrack.Application.Interfaces;

namespace PharmaTrack.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminName = User.FindFirstValue(ClaimTypes.Name) ?? "System Administrator";
        
        var response = await _userService.CreateUserAsync(dto, adminName);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUserAsync();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var response = await _userService.DeleteUserAsync(id);

        if (!response.Success)
            return NotFound(response);
        
        return Ok(response);
    }
}
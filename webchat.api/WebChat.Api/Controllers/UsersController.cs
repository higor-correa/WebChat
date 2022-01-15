namespace WebChat.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebChat.Application.Services.Users;
using WebChat.Domain.Users.DTOs;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserFacade _userFacade;

    public UsersController(IUserFacade userFacade)
    {
        _userFacade = userFacade;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateUserDTO userDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        await _userFacade.CreateAsync(userDTO);

        return Created(string.Empty, userDTO);
    }
}

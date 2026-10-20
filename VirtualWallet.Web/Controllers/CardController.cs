using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Business.DTOs.Cards;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;

namespace VirtualWallet.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCards()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _cardService.GetUserCardsAsync(userId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCard(int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            return Ok(await _cardService.GetByIdAsync(id, userId));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCard([FromBody] CardCreateDto cardDto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            var card = await _cardService.AddCardAsync(userId, cardDto);
            return CreatedAtAction(nameof(GetCard), new { id = card.Id }, card);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidUserInputException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCard(int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            await _cardService.DeleteCardAsync(id, userId);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id:int}/primary")]
    public async Task<IActionResult> SetPrimaryCard(int id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        //204 nocontent
        try
        {
            await _cardService.SetPrimaryCardAsync(id, userId);
            return NoContent();
        }
        //404
        catch (Exception e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        return User.Identity?.IsAuthenticated == true
               && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}

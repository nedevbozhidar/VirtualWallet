using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Business.DTOs.Transfers;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;

namespace VirtualWallet.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/transfers")]
public class TransferController : ControllerBase
{
    private readonly ITransferService _transferService;

    public TransferController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpPost("fund")]
    public async Task<IActionResult> FundWallet([FromBody] TransferCreateDto transferCreateDto)
    {
        if (!TryGetUserId(out int userId)) return Unauthorized();

        try
        {
            return Ok(await _transferService.FundWalletFromCardAsync(userId, transferCreateDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidUserInputException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        return User.Identity?.IsAuthenticated == true
               && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Business.DTOs.Wallets;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;

namespace VirtualWallet.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/wallet")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallets()
    {
        if (!TryGetUserId(out int userId)) return Unauthorized();
        return Ok(await _walletService.GetUserWalletsAsync(userId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWallet(int id)
    {
        if (!TryGetUserId(out int userId)) return Unauthorized();

        try
        {
            return Ok(await _walletService.GetByIdAsync(id, userId));
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
    public async Task<IActionResult> AddWallet([FromBody] WalletCreateDto walletCreateDto)
    {
        if (!TryGetUserId(out int userId)) return Unauthorized();

        try
        {
            var wallet = await _walletService.CreateWalletAsync(userId, walletCreateDto);
            return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id }, wallet);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/default")]
    public async Task<IActionResult> SetDefaultWallet(int id)
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _walletService.SetDefaultWalletAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
        catch (EntityNotFoundException ex)
        {
            return  NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWallet(int id)
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _walletService.DeleteWalletAsync(id, userId);
            return NoContent();
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

    [HttpPost("savings")]
    public async Task<IActionResult> AddWalletSavings([FromBody] SavingsWalletCreateDto savingsWalletCreateDto)
    {
        if (!TryGetUserId(out int userId)) return Unauthorized();

        try
        {
            var wallet = await _walletService.CreateSavingsWalletAsync(userId, savingsWalletCreateDto);
            return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id }, wallet);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
        catch (InvalidUserInputException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InsufficientFundsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        return User.Identity?.IsAuthenticated == true
               && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}

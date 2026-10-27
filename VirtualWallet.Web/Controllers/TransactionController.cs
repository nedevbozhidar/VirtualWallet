using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Business.DTOs.Transactions;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;

namespace VirtualWallet.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public  TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("preview")]
    public async Task<IActionResult> PreviewTransaction([FromBody] TransactionCreateDto transactionCreateDto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            return Ok(await _transactionService.PreviewTransactionAsync(userId, transactionCreateDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidUserInputException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch(BlockedAccountException)
        {
            return Forbid();
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteTransaction([FromBody] TransactionCreateDto transactionCreateDto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            return Ok(await _transactionService.ExecuteTransactionAsync(userId, transactionCreateDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidUserInputException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InsufficientFundsException ex)
        {
            return BadRequest(new  { message = ex.Message });
        }
        catch (BlockedAccountException)
        {
            return Forbid();
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
    }

    [HttpGet("{transactionId:int}")]
    public async Task<IActionResult> GetById(int transactionId)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        bool isAdmin = User.IsInRole("Admin");

        try
        {
            return Ok(await _transactionService.GetByIdAsync(transactionId, userId, isAdmin));
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

    [HttpGet]
    public async Task<IActionResult> GetFilteredTransactions([FromQuery] TransactionFilterDto filterDto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        filterDto.UserId = userId;
        return Ok(await _transactionService.GetFilteredTransactionsAsync(filterDto));

    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        return User.Identity?.IsAuthenticated == true
               && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}

using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DummyApi.DTOs;

namespace VirtualWallet.DummyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WithdrawalsController : ControllerBase
{
    [HttpPost]
    public IActionResult Withdraw([FromBody] WithdrawalRequestDto request)
    {
        if (request.ExpirationDate <= DateTime.UtcNow)
        {
            return BadRequest(new WithdrawalResponseDto()
            {
                IsSuccess = false,
                Message = "The card has expired."
            });
        }

        bool isApproved = Random.Shared.Next(1, 100) <= 80;

        if (isApproved)
        {
            return Ok(new WithdrawalResponseDto()
            {
                Id = Random.Shared.Next(1000, 99999),
                IsSuccess = true,
                Message = "Withdrawal successfully approved."
            });
        }

        return Ok(new WithdrawalResponseDto()
        {
            Id = Random.Shared.Next(1000, 99999),
            IsSuccess = false,
            Message = "Transaction declined by card issuer."
        });
    }
}
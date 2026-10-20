using Microsoft.AspNetCore.DataProtection;
using VirtualWallet.Data.Security;

namespace VirtualWallet.Web.Security;

public sealed class CardDataProtector : ICardDataProtector
{
    private readonly IDataProtector _protector;
    public CardDataProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("VirtualWallet.StoredCardNumber.v1");
    }

    public string Protect(string cardNumber) => _protector.Protect(cardNumber);

    public string Unprotect(string protectedCardNumber) => _protector.Unprotect(protectedCardNumber);

}

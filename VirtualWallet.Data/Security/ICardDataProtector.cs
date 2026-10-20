namespace VirtualWallet.Data.Security;

public interface ICardDataProtector
{
    string Protect(string cardNumber);
    string Unprotect(string protectedCardNumber);
}

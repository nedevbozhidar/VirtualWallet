namespace VirtualWallet.Business.Exceptions;

public class BlockedAccountException : ApplicationException
{
    public BlockedAccountException(string message)
        : base(message)
    {
    }
}

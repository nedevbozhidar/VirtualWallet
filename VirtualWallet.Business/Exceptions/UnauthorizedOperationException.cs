namespace VirtualWallet.Business.Exceptions;

public class UnauthorizedOperationException : ApplicationException
{
    public UnauthorizedOperationException(string message)
        : base(message)
    {
    }
}

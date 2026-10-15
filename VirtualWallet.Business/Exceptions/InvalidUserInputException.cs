namespace VirtualWallet.Business.Exceptions;

public class InvalidUserInputException : ApplicationException
{
    public InvalidUserInputException(string message)
        : base(message)
    {
    }
}

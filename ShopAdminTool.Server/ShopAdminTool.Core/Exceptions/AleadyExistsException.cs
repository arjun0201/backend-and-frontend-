namespace ShopAdminTool.Core.Exceptions;

public class AleadyExistsException : Exception
{
    public AleadyExistsException() : base()
    {
    }

    public AleadyExistsException(string message) : base(message)
    {
    }

}

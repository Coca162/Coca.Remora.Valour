namespace Coca.Remora.Valour;

public class ValourTokenNotFoundException : Exception
{
    public ValourTokenNotFoundException(string message): base(message) { }
}

public class ValourSetUpFailureException : Exception
{
    public ValourSetUpFailureException(string message) : base(message) { }
}
using FieldServiceManagement.Enum;

public class InvalidLoginException : Exception
{
    public InvalidLoginReason Reason { get; }
    public InvalidLoginException(InvalidLoginReason reason) : base(reason.ToString())
        => Reason = reason;
}
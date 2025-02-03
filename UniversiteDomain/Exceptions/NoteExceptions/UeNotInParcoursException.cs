namespace UniversiteDomain.Exceptions.NoteExceptions;

public class UeNotInParcoursException : Exception
{
    public UeNotInParcoursException(string message) : base(message) { }
    public UeNotInParcoursException(string message, Exception inner) : base(message, inner) { }
}
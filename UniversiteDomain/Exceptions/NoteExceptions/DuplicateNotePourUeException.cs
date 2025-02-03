namespace UniversiteDomain.Exceptions.NoteExceptions;

public class DuplicateNotePourUeException : Exception
{
    public DuplicateNotePourUeException() : base() { }
    public DuplicateNotePourUeException(string message) : base(message) { }
    public DuplicateNotePourUeException(string message, Exception inner) : base(message, inner) { }
}
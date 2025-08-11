using System;

public class SearchServiceException : Exception
{
    public SearchServiceException(string message, Exception innerException)
        : base(message, innerException) { }
}

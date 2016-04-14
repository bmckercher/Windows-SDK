namespace MASFoundation
{
    public enum ErrorKind : int
    {
        None = 0,
        InvalidConfigurationFile = 1,
        UnableToLoadConfigurationFile,
        HttpRequestError,
    }
}

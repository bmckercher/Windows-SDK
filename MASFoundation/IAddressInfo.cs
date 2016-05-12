namespace MASFoundation
{
    /// <summary>
    /// Basic address information of the user
    /// </summary>
    public interface IAddressInfo
    {
        /// <summary>
        /// Their region
        /// </summary>
        string Region { get; }

        /// <summary>
        /// Their country
        /// </summary>
        string Country { get; }
    }
}

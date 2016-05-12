namespace MASFoundation
{
    /// <summary>
    /// User information
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// Sub
        /// </summary>
        string Sub { get; }

        /// <summary>
        /// First name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Family or last name
        /// </summary>
        string FamilyName { get; }

        /// <summary>
        /// Nickname
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// Perferred username
        /// </summary>
        string PerferredUsername { get; }

        /// <summary>
        /// Email address
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Phone number
        /// </summary>
        string Phone { get; }

        /// <summary>
        /// Address
        /// </summary>
        IAddressInfo Address { get; }
    }
}

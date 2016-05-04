namespace MASFoundation
{
    public interface IUserInfo
    {
        string Sub { get; }
        string Name { get; }
        string FamilyName { get; }
        string Nickname { get; }
        string PerferredUsername { get; }
        string Email { get; }
        string Phone { get; }
        IAddressInfo Address { get; }
    }
}

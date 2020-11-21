namespace Etimo.Id.Abstractions
{
    public interface IPasswordHasher
    {
        string Hash(string text);
        bool Verify(string text, string hash);
    }
}

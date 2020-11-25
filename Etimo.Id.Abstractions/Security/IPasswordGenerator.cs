namespace Etimo.Id.Abstractions
{
    public interface IPasswordGenerator
    {
        string Generate(int length);
    }
}

namespace Etimo.Id.Client
{
    public interface IEtimoIdClient
        : IEtimoIdApplicationClient,
            IEtimoIdOAuthClient,
            IEtimoIdUserClient { }
}

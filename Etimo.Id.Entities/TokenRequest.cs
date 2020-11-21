namespace Etimo.Id.Entities
{
    public class TokenRequest
    {
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}

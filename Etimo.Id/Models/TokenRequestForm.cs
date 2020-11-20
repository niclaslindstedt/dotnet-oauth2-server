// ReSharper disable InconsistentNaming
namespace Etimo.Id.Models
{
    public class TokenRequestForm
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}

// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationSecretResponseDto : ApplicationResponseDto
    {
        public string client_secret { get; set; }

        public new static ApplicationSecretResponseDto FromApplication(Application application)
        {
            var response = ApplicationResponseDto.FromApplication(application) as ApplicationSecretResponseDto;
            if (response != null)
            {
                response.client_secret = application.ClientSecret;
            }

            return response;
        }
    }
}

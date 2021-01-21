using Etimo.Id.Client;
using Etimo.Id.Dtos;
using Etimo.Id.Exceptions;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Web.Pages
{
    public partial class ApplicationsPage
    {
        [Inject]
        public IEtimoIdApplicationClient ApplicationClient { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public List<ApplicationResponseDto> Applications { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ResponseDto<List<ApplicationResponseDto>> response = await ApplicationClient.GetApplicationsAsync();
                if (response.Success) { Applications = response.Data; }
            }
            catch (UnauthorizedException) { NavigationManager.NavigateTo("/login"); }
        }
    }
}

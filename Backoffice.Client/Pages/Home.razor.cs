using Backoffice.Client.ServiceProxies.Commands.Administrators;
using Microsoft.AspNetCore.Components;

namespace Backoffice.Client.Pages;

public partial class Home
{
    // [Inject] public CreateDefaultAdministratorCommandProxy CreateDefaultAdministratorCommandProxy { get; set; }


    protected override async Task OnInitializedAsync()
    {
        // await CreateDefaultAdministratorCommandProxy.Execute();
    }
}
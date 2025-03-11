using Backoffice.Client.ServiceProxies.Commands.Administrators;
using Backoffice.Client.ServiceProxies.Queries.Administrators;
using Backoffice.Client.ServiceProxies.Queries.Administrators.Models;
using Microsoft.AspNetCore.Components;

namespace Backoffice.Client.Pages;

public partial class Home
{
    [Inject] public CreateDefaultAdministratorCommandProxy CreateDefaultAdministratorCommandProxy { get; set; }
    [Inject] public GetAllAdministratorsQueryProxy  GetAllAdministratorsQueryProxy { get; set; }

    private  IEnumerable<AdministratorModel>? administrators;
    

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("OnInitializedAsync");
        try
        {
            await CreateDefaultAdministratorCommandProxy.Execute();

            Console.WriteLine("Trying to create default administrator");
            
            administrators = await GetAllAdministratorsQueryProxy.Execute();
            if (administrators != null)
            {
                Console.WriteLine($"{administrators.Count()} administrators");
            }
            else
            {
                Console.WriteLine("No administrators found");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR!!!!!!!");
            Console.WriteLine(e.Message);
        }
        
        
        Console.WriteLine("End OnInitializedAsync");

    }
}
using System.Collections;
using Backoffice.Client.ServiceProxies.Queries.Administrators.Models;

namespace Backoffice.Client.ServiceProxies.Queries.Administrators;

public partial class GetAllAdministratorsQueryProxy : ProxyBaseNoInput<IEnumerable<AdministratorModel>>
{
    public GetAllAdministratorsQueryProxy(Executor executor) : base(executor)
    {
    }
}
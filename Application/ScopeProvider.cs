namespace Application;

public class ScopeProvider
{
    private Guid? scopeId;
    private DateTime? creationDt;
    
    public Guid ScopeId
    {
        get { if(!scopeId.HasValue)
            {
                scopeId = Guid.NewGuid();
                creationDt=DateTime.Now;
            }
            return scopeId.Value;
        }
    }
    
    public DateTime CreateDt => creationDt.Value;
}
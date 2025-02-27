namespace Persistence;

public interface IChangesSaver
{
    void SaveChanges();

    Task SaveChangesAsync();
}
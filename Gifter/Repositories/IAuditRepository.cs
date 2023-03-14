namespace Gifter.Repositories
{
    public interface IAuditRepository
    {
        void Add(string tableName, string operation, string oldValue, string newValue);
    }
}
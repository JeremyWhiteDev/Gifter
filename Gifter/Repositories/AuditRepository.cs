using Gifter.Utils;

namespace Gifter.Repositories;

public class AuditRepository : BaseRepository, IAuditRepository
{
    public AuditRepository(IConfiguration configuration) : base(configuration) { }

    public void Add(string tableName, string operation, string oldValue, string newValue)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO [dbo].[Audit]
                                               ([TableName]
                                               ,[Operation]
                                               ,[DateTime]
                                               ,[OldValue]
                                               ,[NewValue])
                                         VALUES
                                               (@TableName,
                                               @Operation,
                                               @DateTime,
                                               @OldValue,
                                               @NewValue)";
                DbUtils.AddParameter(cmd, "@TableName", tableName);
                DbUtils.AddParameter(cmd, "@Operation", operation);
                DbUtils.AddParameter(cmd, "@DateTime", DateTime.Now);
                DbUtils.AddParameter(cmd, "@OldValue", oldValue);
                DbUtils.AddParameter(cmd, "@NewValue", newValue);
                cmd.ExecuteNonQuery();

            }
        }
    }
}

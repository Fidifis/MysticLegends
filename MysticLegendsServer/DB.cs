using Npgsql;

namespace MysticLegendsServer
{
    public class DB: IDisposable, IAsyncDisposable
    {
        public static DB? Connection { get; private set; } = null;
        private NpgsqlDataSource? dbConnection;

        public static void OpenConnection(string connectionString)
        {
            Connection = new DB();
            Connection.Connect(connectionString);
        }

        public void Connect(string connectionString)
        {
            dbConnection = NpgsqlDataSource.Create(connectionString);
            //dbConnection = await dataSource.OpenConnectionAsync();
        }

        public async Task<NpgsqlDataReader> QueryReader(string sql)
        {
            await using var command = dbConnection!.CreateCommand(sql);
            return await command.ExecuteReaderAsync();
        }

        public async Task<List<List<string>>> Query(string sql)
        {
            await using var reader = await QueryReader(sql);
            var data = new List<List<string>>();
            while (await reader.ReadAsync())
            {
                var columnCount = reader.FieldCount;
                data.Add(new List<string>(columnCount));

                for (int column = 0; column < columnCount; column++)
                {
                    string v;
                    var type = reader.GetFieldType(column);

                    if (type == typeof(string))
                        v = reader.GetString(column);
                    else if (type == typeof(int))
                        v = reader.GetInt32(column).ToString();
                    else
                        throw new NotImplementedException("Unknow database type");

                    data[^1].Add(v);
                }
            }
            return data;
        }

        public async Task NonQuery(string sql)
        {
            await using var command = dbConnection!.CreateCommand(sql);
            await command.ExecuteNonQueryAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (Connection == this)
                Connection = null;
            dbConnection?.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            if (Connection == this)
                Connection = null;
            return dbConnection?.DisposeAsync() ?? default;
        }

        ~DB()
        {
            dbConnection?.Dispose();
        }
    }
}

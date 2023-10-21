using Npgsql;
using System.Reflection.PortableExecutable;

namespace MysticLegendsServer
{
    public class DB: IDisposable, IAsyncDisposable
    {
        public static DB? Connection { get; private set; } = null;
        private NpgsqlConnection? dbConnection;

        public static async void OpenConnection(string connectionString)
        {
            Connection = new DB();
            await Connection.Connect(connectionString);
        }

        public async Task Connect(string connectionString)
        {
            var dataSource = NpgsqlDataSource.Create(connectionString);
            dbConnection = await dataSource.OpenConnectionAsync();
        }

        public async Task<NpgsqlDataReader> QueryReader(string sql)
        {
            await using var command = new NpgsqlCommand(sql, dbConnection);
            return  await command.ExecuteReaderAsync();
        }

        public async Task<List<List<string>>> Query(string sql)
        {
            await using var command = new NpgsqlCommand(sql, dbConnection);
            await using var reader = await command.ExecuteReaderAsync();
            var data = new List<List<string>>();
            while (reader.Read())
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
            await using var command = new NpgsqlCommand(sql, dbConnection);
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

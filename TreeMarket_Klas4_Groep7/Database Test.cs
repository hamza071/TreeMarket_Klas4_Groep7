// File: DatabaseTest.cs
// Requires NuGet: Microsoft.Data.SqlClient
// dotnet add package Microsoft.Data.SqlClient

using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SchoolDbTest
{
    public static class DatabaseTest
    {
        // Pas desnoods aan: gebruik lokaal "localhost,1433" ipv DuckDNS.
        private const string Server = "schoolopd.duckdns.org,51433";
        private const string DatabaseName = "School";
        private const string User = "sa";
        private const string Password = "Hamza_123!";

        // Encryptie aan + TrustServerCertificate voor self-signed cert
        private static readonly string MasterConn =
            $"Server={Server};Database=master;User Id={User};Password={Password};Encrypt=True;TrustServerCertificate=True;";

        private static readonly string DbConn =
            $"Server={Server};Database={DatabaseName};User Id={User};Password={Password};Encrypt=True;TrustServerCertificate=True;";

        public static async Task Main()
        {
            try
            {
                Console.WriteLine("== Step 1: Connect to master ==");
                await UsingConnection(MasterConn, async conn =>
                {
                    Console.WriteLine("Connected to master");

                    Console.WriteLine("== Step 2: Create database if not exists ==");
                    var createDbSql = $@"
IF DB_ID('{DatabaseName}') IS NULL
BEGIN
    CREATE DATABASE [{DatabaseName}];
END";
                    await ExecNonQuery(conn, createDbSql);
                    Console.WriteLine($"Database '{DatabaseName}' ready.");
                });

                Console.WriteLine("== Step 3: Connect to School DB ==");
                await UsingConnection(DbConn, async conn =>
                {
                    Console.WriteLine($"Connected to {DatabaseName}");

                    Console.WriteLine("== Step 4: Create demo table ==");
                    var createTableSql = @"
IF OBJECT_ID('dbo.Demo','U') IS NULL
BEGIN
    CREATE TABLE dbo.Demo(
        Id INT NOT NULL PRIMARY KEY,
        Note NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END";
                    await ExecNonQuery(conn, createTableSql);
                    Console.WriteLine("Table dbo.Demo ready.");

                    Console.WriteLine("== Step 5: Clean & Insert sample rows ==");
                    await ExecNonQuery(conn, "DELETE FROM dbo.Demo;");
                    await ExecNonQuery(conn, "INSERT INTO dbo.Demo(Id, Note) VALUES (1, @p0);", ("@p0", "Hello"));
                    await ExecNonQuery(conn, "INSERT INTO dbo.Demo(Id, Note) VALUES (2, @p0);", ("@p0", "World"));
                    Console.WriteLine("Inserted 2 rows.");

                    Console.WriteLine("== Step 6: Query rows ==");
                    await QueryAndPrint(conn, "SELECT Id, Note, CreatedAt FROM dbo.Demo ORDER BY Id;");

                    Console.WriteLine("== Step 7: Update with parameter ==");
                    await ExecNonQuery(conn, "UPDATE dbo.Demo SET Note = @p0 WHERE Id = @p1;", ("@p0", "Updated"), ("@p1", 2));
                    await QueryAndPrint(conn, "SELECT Id, Note, CreatedAt FROM dbo.Demo ORDER BY Id;");

                    Console.WriteLine("== Step 8: Delete a row ==");
                    await ExecNonQuery(conn, "DELETE FROM dbo.Demo WHERE Id = @p0;", ("@p0", 1));
                    await QueryAndPrint(conn, "SELECT Id, Note, CreatedAt FROM dbo.Demo ORDER BY Id;");

                    Console.WriteLine("== OK ==");
                });
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine("SQL ERROR:");
                Console.Error.WriteLine(ex.ToString());
                Environment.ExitCode = 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR:");
                Console.Error.WriteLine(ex.ToString());
                Environment.ExitCode = 1;
            }
        }

        private static async Task UsingConnection(string connString, Func<SqlConnection, Task> action)
        {
            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync();
            await action(conn);
        }

        private static async Task ExecNonQuery(SqlConnection conn, string sql, params (string name, object value)[] p)
        {
            await using var cmd = new SqlCommand(sql, conn);
            foreach (var (name, value) in p)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task QueryAndPrint(SqlConnection conn, string sql, params (string name, object value)[] p)
        {
            await using var cmd = new SqlCommand(sql, conn);
            foreach (var (name, value) in p)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

            await using var r = await cmd.ExecuteReaderAsync();
            var hasRows = false;
            while (await r.ReadAsync())
            {
                hasRows = true;
                var id = r.GetInt32(0);
                var note = r.GetString(1);
                var created = r.GetDateTime(2);
                Console.WriteLine($"Id={id} | Note='{note}' | CreatedAt={created:O}");
            }
            if (!hasRows) Console.WriteLine("<no rows>");
        }
    }
}

namespace TaskManager.Infrastructure.Respositories;

using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManager.Domain.Entities;

public class UserRepository(IConfiguration config) : IUserRepository
{
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;

    public User GetUserByUsername(string username)
    {
        using var connection = new SqlConnection(_connectionString);
        return connection.QueryFirstOrDefault<User>(
            "SELECT * FROM Users WHERE Username = @Username",
            new { Username = username }) ?? new User();
    }
    public void AddUser(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @PasswordHash, @Role)";
        connection.Execute(query, user);
    }
}


using Microsoft.AspNetCore.Http;
using ConfigurationLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

using Dapper;

namespace ConfigurationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=DynamicConfigDb;Trusted_Connection=True;";

        [HttpGet("{applicationName}")]
        public IActionResult GetByAppName(string applicationName)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT * FROM Configurations WHERE ApplicationName = @App";
            var configs = connection.Query<ConfigurationItem>(query, new { App = applicationName });
            return Ok(configs);
        }

        [HttpPost]
        public IActionResult Add([FromBody] ConfigurationItem item)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"INSERT INTO Configurations (Name, Type, Value, IsActive, ApplicationName)
                             VALUES (@Name, @Type, @Value, @IsActive, @ApplicationName)";
            connection.Execute(query, item);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ConfigurationItem item)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"UPDATE Configurations 
                             SET Name=@Name, Type=@Type, Value=@Value, IsActive=@IsActive, ApplicationName=@ApplicationName 
                             WHERE Id=@Id";
            item.Id = id;
            connection.Execute(query, item);
            return Ok();
        }
    }
}

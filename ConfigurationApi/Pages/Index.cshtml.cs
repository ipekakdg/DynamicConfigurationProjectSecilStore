using ConfigurationLibrary.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace ConfigurationApi.Pages
{
    public class IndexModel : PageModel
    {
        public List<ConfigurationItem> Configs { get; set; } = new();

        [BindProperty]
        public ConfigurationItem NewConfig { get; set; } = new();

        [BindProperty]
        public int Id { get; set; } 

        public void OnGet()
        {
            LoadConfigs();
            System.Diagnostics.Debug.WriteLine(">>> OnGet metodu çalıştı");
        }

        public IActionResult OnPostAdd()
        {
            System.Diagnostics.Debug.WriteLine(">>> OnPostAdd metodu giriş yaptı");

            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine(">>> ModelState geçerli değil, hatalar listeleniyor:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($">>> Hata: {key} -> {error.ErrorMessage}");
                    }
                }

                return Page(); 
            }

            System.Diagnostics.Debug.WriteLine($"Name: {NewConfig.Name}");
            System.Diagnostics.Debug.WriteLine($"Type: {NewConfig.Type}");
            System.Diagnostics.Debug.WriteLine($"Value: {NewConfig.Value}");
            System.Diagnostics.Debug.WriteLine($"ApplicationName: {NewConfig.ApplicationName}");
            System.Diagnostics.Debug.WriteLine($"IsActive: {NewConfig.IsActive}");


            try
            {
                using var connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=DynamicConfigDb;Trusted_Connection=True;");
                string insertQuery = @"INSERT INTO Configurations (Name, Type, Value, IsActive, ApplicationName)
                                       VALUES (@Name, @Type, @Value, @IsActive, @ApplicationName)";

                connection.Execute(insertQuery, NewConfig);

                System.Diagnostics.Debug.WriteLine(">>> OnPostAdd metodu çalıştı");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(">>> HATA: " + ex.Message);
            }
            var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            foreach (var kvp in formData)
            {
                System.Diagnostics.Debug.WriteLine($"Form Key: {kvp.Key}, Value: {kvp.Value}");
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            using var connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=DynamicConfigDb;Trusted_Connection=True;");
            string deleteQuery = "UPDATE Configurations SET IsActive = 0 WHERE Id = @Id";
            connection.Execute(deleteQuery, new { Id });

            return RedirectToPage(); 
        }

        private void LoadConfigs()
        {
            using var connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=DynamicConfigDb;Trusted_Connection=True;");
            string selectQuery = "SELECT * FROM Configurations WHERE IsActive = 1";
            Configs = connection.Query<ConfigurationItem>(selectQuery).ToList();
        }
    }
}
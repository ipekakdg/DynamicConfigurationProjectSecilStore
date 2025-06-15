using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;

namespace ConfigurationLibrary
{
    public class ConfigurationReader : IDisposable
    {
        private readonly string _applicationName;
        private readonly string _connectionString;
        private readonly int _refreshInterval;
        private Dictionary<string, object> _configCache;
        private readonly object _cacheLock = new();
        private CancellationTokenSource _cts;

        public ConfigurationReader(string applicationName, string connectionString, int refreshInterval)
        {
            _applicationName = applicationName;
            _connectionString = connectionString;
            _refreshInterval = refreshInterval;
            _configCache = new Dictionary<string, object>();
            _cts = new CancellationTokenSource();

           
            _ = RefreshLoopAsync(_cts.Token);
        }

        private async Task RefreshLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await LoadConfigurationsAsync();
                try
                {
                    await Task.Delay(_refreshInterval, token);
                }
                catch (TaskCanceledException)
                {
                   
                    break;
                }
            }
        }

        private async Task LoadConfigurationsAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"
                    SELECT Name, Value, Type 
                    FROM Configurations 
                    WHERE IsActive = 1 AND ApplicationName = @AppName";

                var results = await connection.QueryAsync<(string Name, string Value, string Type)>(
                    query, new { AppName = _applicationName });

                var newCache = new Dictionary<string, object>();

                foreach (var config in results)
                {
                    object converted = ConvertToType(config.Value, config.Type);
                    newCache[config.Name] = converted;
                }

                UpdateCache(newCache);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadConfigurationsAsync hata: {ex.Message}");
            }
        }

        private void UpdateCache(Dictionary<string, object> newCache)
        {
            lock (_cacheLock)
            {
                _configCache = newCache;
            }
        }

        private object ConvertToType(string value, string type)
        {
            return type.ToLower() switch
            {
                "int" => int.Parse(value),
                "bool" => bool.Parse(value),
                "double" => double.Parse(value),
                "string" => value,
                _ => throw new NotSupportedException($"Desteklenmeyen tip: {type}")
            };
        }

        public T GetValue<T>(string key)
        {
            lock (_cacheLock)
            {
                if (_configCache.TryGetValue(key, out var value))
                    return (T)value;
            }
            throw new KeyNotFoundException($"Anahtar bulunamadı: {key}");
        }


        public void Dispose()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null!;
            }
        }
    }
}

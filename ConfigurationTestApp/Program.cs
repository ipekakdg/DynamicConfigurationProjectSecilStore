using ConfigurationLibrary;


string connStr = "Server=(localdb)\\MSSQLLocalDB;Database=DynamicConfigDb;Trusted_Connection=True;";
string appName = "SERVICE-A";
int interval = 5000; 

var reader = new ConfigurationReader(appName, connStr, interval);

Console.WriteLine("Veriler yükleniyor...");
Thread.Sleep(1000);

try
{
    string siteName = reader.GetValue<string>("SiteName");
    Console.WriteLine("Site Adı: " + siteName);

    bool isBasketEnabled = reader.GetValue<bool>("IsBasketEnabled");
    Console.WriteLine("Sepet Açık mı?: " + isBasketEnabled);

    int maxItems = reader.GetValue<int>("MaxItemCount");
    Console.WriteLine("Maksimum Ürün: " + maxItems);
}
catch (Exception ex)
{
    Console.WriteLine("HATA: " + ex.Message);
}

Console.WriteLine("\nTest tamamlandı. Çıkmak için bir tuşa bas...");
Console.ReadKey();

# DynamicConfigurationProjectSecilStore

ConfigurationReader
.NET 8 tabanlı, dinamik konfigürasyon yönetimi sağlayan bir yapı.
appsettings.json, web.config gibi geleneksel yapıların yerine kullanılmak üzere geliştirilmiş, veritabanı destekli ve deployment gerektirmeyen bir yapı sunar.

Proje Yapısı

Solution
  •	ConfigurationLibrary      // DLL olarak kullanılabilen yapı
     o	ConfigurationReader.cs
     o	Models/ConfigurationItem.cs

  •	ConfigurationApi          // Razor Pages arayüz
     o	Pages/Index.cshtml
     o	wwwroot/images/secil-logoblack.png

  •	ConfigurationTestApp  

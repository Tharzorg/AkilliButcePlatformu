# Akıllı Bütçe ve Finans Yönetim Platformu

ASP.NET Core (.NET 8) Razor Pages, RESTful Web API, MSSQL Server ve Flutter kullanılarak geliştirilmiş çok platformlu kişisel finans ve fiş gizliliği (EXIF temizleme) platformu.

## Klasör Yapısı
- `/api/ButcePlatformu`: ASP.NET Core Razor Pages yönetim paneli, Web API, Swagger ve EF Core MSSQL katmanı.
- `/mobile/butce_mobil`: Web API üzerinden CRUD ve rol bazlı yetkilendirme yapan Flutter mobil uygulaması.

## Kurulum ve Çalıştırma

### 1. Veritabanı (MSSQL)
Docker üzerinde MSSQL Server başlatılır:
```bash
docker run --platform linux/amd64 -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=GucluSifre2026!' -p 1433:1433 --name mssql_butce -d [mcr.microsoft.com/mssql/server:2022-latest](https://mcr.microsoft.com/mssql/server:2022-latest)
```

### 2. Backend & Web Paneli (`/api/ButcePlatformu`)
Güvenlik nedeniyle bağlantı dizesi repoya dahil edilmemiştir. `api/ButcePlatformu` klasörünün içine `appsettings.Development.json` adında bir dosya oluşturulup aşağıdaki içerik eklenir:
```json
{
  "ConnectionStrings": {
    "VarsayilanBaglanti": "Server=localhost,1433;Database=AkilliButceDb;User Id=sa;Password=GucluSifre2026!;TrustServerCertificate=True;"
  }
}
```

Ardından veritabanı tabloları oluşturulur ve sunucu başlatılır:
```bash
cd api/ButcePlatformu
dotnet ef database update
dotnet run
```
- Web Paneli: `http://localhost:5044`
- Swagger API: `http://localhost:5044/swagger`
- Varsayılan Yönetici: `admin@butce.local` / `Yonetici2026!`

### 3. Mobil Uygulama (`/mobile/butce_mobil`)
```bash
cd mobile/butce_mobil
flutter pub get
flutter run
```
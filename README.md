# IF3250_2022_01_BULETIN_Backend

## Langkah - Langkah Menjalankan Project

1. Buat File "Credentials.cs" di root directory
2. Ganti database username dan password

```
class Credentials
{
    // DB Config
    public const string DB_USERNAME = "<DB_USERNAME>";
    public const string DB_PASSWORD = "<DB_PASSWORD>";
}
```

2. Jalankan command "dotnet restore"
3. Jalankan command "dotnet ef database update"  
   jika belum ada "ef" tool, maka jalankan command "dotnet tool install dotnet-ef"
4. Jalankan command "dotnet run"

## Cara migrasi database

1. Jalankan command "dotnet ef migrations <nama_migrasi>"  
   jika belum ada "ef" tool, maka jalankan command "dotnet tool install dotnet-ef"
2. Jalankan command "dotnet ef database update" untuk mengupdate database

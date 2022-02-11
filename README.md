# IF3250_2022_01_BULETIN_Backend

## Langkah - Langkah Menjalankan Server

1. Buat File "Credentials.cs" di directory _src/_
2. Ganti database username dan password

```
class Credentials
{
    // DB Config
    public const string DB_USERNAME = "<DB_USERNAME>";
    public const string DB_PASSWORD = "<DB_PASSWORD>";
}
```

2. Mengupdate project (download package)

```
dotnet restore
```

3. _Jika belum ada_ "ef" tool (untuk migrasi)

```
dotnet tool install dotnet-ef
```

4. Mengupdate database (pastikan postgresql sudah jalan)

```
dotnet ef database update
```

4. Menjalankan server

```
dotnet run
```

## Cara migrasi database

1. Membuat atau menambahkan migrasi (ganti <nama_migrasi>)

```
dotnet ef migrations <nama_migrasi> -o src/Migrations
```

2. Mengupdate database

```
dotnet ef database update
```

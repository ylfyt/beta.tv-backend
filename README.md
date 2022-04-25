# IF3250_2022_01_BULETIN_Backend

## Cara Menggunakan Docker

1. Jika pertama kali menjalankan server dengan docker, jalan script

```
./update-server-containers.bat
```

or

```
./start-server-containers.bat
```

2. Jika sudah mengedit src program, untuk mengupdate container jalankan script

```
./update-server-containers.bat
```

3. Untuk stop atau start containers, jalankan script

```
./stop-server-containers.bat
```

```
./start-server-containers.bat
```

## Langkah - Langkah Menjalankan Server

1. Duplicate file **".env.example"** pada directory yang sama, lalu hilangkan kata **".example"** pada file hasil duplicate, sehingga nama file menjadi **".env"**
2. Ubah nilai credential pada file **".env"** tersebut
3. Mengupdate project (download package, jika menggunakan visual studio harusnya otomatis)

```
dotnet restore
```

4. **Jika belum ada** "ef" tool (untuk migrasi)

```
dotnet tool install --global dotnet-ef
```

5. Mengupdate database (pastikan postgresql sudah jalan)

```
dotnet ef database update
```

6. Menjalankan server

```
dotnet run
```

## Cara migrasi database

1. Membuat atau menambahkan migrasi (ganti <nama_migrasi>)

```
dotnet ef migrations add <nama_migrasi> -o src/Migrations
```

2. Mengupdate database

```
dotnet ef database update
```

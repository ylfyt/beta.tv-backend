public static class Constants
{
    public const string DB_HOST = "localhost";
    public const string DB_NAME = "buletin.id";
    public static string connectionString { get; } = $"Host=localhost;Username=postgres;Password=postgres;Database=buletin.id";

    public static string connectionStringHero { get; } = $"Host=ec2-3-209-124-113.compute-1.amazonaws.com;Username=eteamlhldbyoma;Password=503f82c4a821f26b0d5795961eb4f0ce3d6bae6f4a06f27d13bcf75c3464c239;Database=d3pfln8am6ie6r;Port=5432";

    public static string JWT_SECRET_KEY { get; } = "secret key dkasjlkdasjmkldasmkdamslkdmaskmd";

    public const string YOUTUBE_CHANNEL_ID = "UCSVFeUogGX4M_N4l3zjrhFQ";

}
public static class Constants
{
    public const string DB_HOST = "localhost";
    public const string DB_NAME = "buletin.id";
    public static string connectionString { get; } = $"Host={Constants.DB_HOST};Username={Credentials.DB_USERNAME};Password={Credentials.DB_PASSWORD};Database={Constants.DB_NAME}";

    public static string JWT_SECRET_KEY { get; } = "secret key dkasjlkdasjmkldasmkdamslkdmaskmd";

    public const string YOUTUBE_CHANNEL_ID = "UCSVFeUogGX4M_N4l3zjrhFQ";

}
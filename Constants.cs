class Constants
{
    public const string DB_HOST = "localhost";
    public const string DB_NAME = "buletin.id";
    public static string connectionString { get; } = $"Host={Constants.DB_HOST};Username={Credentials.DB_USERNAME};Password={Credentials.DB_PASSWORD};Database={Constants.DB_NAME}";
}
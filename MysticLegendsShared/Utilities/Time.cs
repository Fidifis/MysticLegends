namespace MysticLegendsShared.Utilities
{
    public static class Time
    {
        public static DateTime Current => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
    }
}

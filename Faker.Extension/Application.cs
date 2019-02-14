using System;
namespace Faker
{
    public static class Application
    {
        public static string Version() => $"{RandomNumber.Next(1, 10)}.{RandomNumber.Next(0, 10)}.{RandomNumber.Next(0, 100)}.{RandomNumber.Next(0, 9000)}";
    }
}

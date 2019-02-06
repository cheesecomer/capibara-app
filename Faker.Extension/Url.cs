namespace Faker
{
    public static class Url
    {
        public static string Root() => $"https://{Internet.DomainName()}.com/";

        public static string Image() => $"https://{Internet.DomainName()}.com/images/{System.Guid.NewGuid()}.png";
    }
}

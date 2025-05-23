namespace Shared.Misc
{
    public static class SourceUrl
    {
        private const string url = "https://raw.githubusercontent.com/biggiko/nasa-dataset/refs/heads/main/y77d-th95.json";

        public static string GetUrl()
        {
            return url;
        }
    }
}
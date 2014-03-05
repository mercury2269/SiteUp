namespace SiteUp
{
    public class LocalFile
    {
        public LocalFile()
        {
        }

        public LocalFile(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
        public string ETag { get; set; }
    }
}
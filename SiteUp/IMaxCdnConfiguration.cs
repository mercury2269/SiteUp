namespace SiteUp
{
    public interface IMaxCdnConfiguration
    {
        string[] PurgeExcludedExtensions { get; set; }
        string Alias { get; set; }
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        string Zone { get; set; }
        int RequestTimeout { get; set; }
    }
}
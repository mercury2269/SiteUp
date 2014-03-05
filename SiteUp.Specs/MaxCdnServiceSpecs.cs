using System.Collections.Generic;
using Machine.Specifications;
using Moq;
using SiteUp;

namespace S3Publish.Specs
{
    [Subject(typeof(MaxCdnService))]
    public class when_files_with_excluded_extensions_are_preset
    {
        public static MaxCdnService Service;
        public static Mock<IMaxCdnConfiguration> Configuration;
        public static List<string> Keys = new List<string>();

        private Establish context = () =>
        {
            var extensionsToExclude = new[] {".md", ".config"};
            Keys.Add("app.config");
            Keys.Add("retained.xml");
            Keys.Add("retained2.html");
            Keys.Add("posts/README.md");


            Configuration = new Mock<IMaxCdnConfiguration>();
            Configuration.Setup(p => p.PurgeExcludedExtensions).Returns(extensionsToExclude);
            Service = new MaxCdnService(Configuration.Object);
        };

        Because of = () => Service.PurgeFiles(Keys);

        private It should_purge_only_files_that_dont_match_excluded_extensions =
            () => Keys.ShouldContainOnly(new[] {"retained.xml", "retained2.html"});
    }
}
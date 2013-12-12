using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using SiteUp.Mono.Options;

namespace SiteUp
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = null;
            bool showHelp = false;
            var options = new OptionSet()
            {
                {"p|path=", "Path to folder to publish", p => path = p},
                {"h|help", "show this message and exit", v => showHelp = v != null},
            };

            var value = ConfigurationManager.AppSettings["AWSAccessKey"];

            try
            {
                options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.WriteLine (ex.Message);
                Console.WriteLine ("Try `siteup --help' for more information.");
                return;
            }

            if (showHelp || path == null)
            {
                ShowHelp(options);
                return;
            }

            if (!Directory.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to find {0} directory", path);
                return;
            }

            var browser = new DirectoryBrowser();
            var localFiles = browser.GetAllFiles(path);

            var amazonService = new AmazonService();
            var s3Objects = amazonService.GetListOfAllObjects();
            var compareResults = new ContentComparer().CompareLocalToS3(localFiles, s3Objects);

            if (NeedToSync(compareResults))
            {
                var filesToSync = compareResults.Where(p => p.Status != CompareStatus.Retained).ToList();
                OutputToConsoleFilesToSync(compareResults);
                amazonService.SyncModifiedFiles(filesToSync, path);
                var maxCdnService = new MaxCdnService();
                Console.WriteLine("Purging Cache");
                maxCdnService.PurgeFiles(filesToSync.Select(p => p.Key).ToList());
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("All up to date");
            }
        }

        private static void ShowHelp(OptionSet options)
        {
            options.WriteOptionDescriptions(Console.Out);
        }

        private static void OutputToConsoleFilesToSync(IEnumerable<CompareResult> compareResults)
        {
            foreach (var compareResult in compareResults)
            {
                Console.WriteLine("{0}: {1}", compareResult.Status, compareResult.Key);
            }
        }

        private static bool NeedToSync(IEnumerable<CompareResult> compareResults)
        {
            return compareResults.Any(p => !p.Status.Equals(CompareStatus.Retained));
        }
    }
}

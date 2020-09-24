using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace DownloadsDirCleaner
{
    class Program
    {
        class Options
        {
            [Option('f', "downloadsFolder", Required = true, HelpText = "The path to the downloads folder")]
            public string DownloadsDirectory { get; set; }

            [Option('d', "daysToKeep", Required = false, HelpText = "The number of days passed before a file will get deleted (defaults to 5 days)")]
            public int DaysToKeep { get; set; } = 5;
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(CleanupDownloadsDirectory)
                .WithNotParsed(HandleParserError);
        }

        static void CleanupDownloadsDirectory(Options opts)
        {
            DateTime today = DateTime.UtcNow;
            string[] files = Directory.GetFiles(opts.DownloadsDirectory);

            foreach (var file in files)
            {
                // get the last date this file was accessed
                DateTime lastAccessed = File.GetLastAccessTime(file);

                // get the number of days that's passed since this file was last accessed
                int daysSinceAccessed = Convert.ToInt32(Math.Abs((lastAccessed - today).TotalDays));

                // if the file hasn't been touched for the optioned days to keep value, delete the file
                if (daysSinceAccessed >= opts.DaysToKeep)
                    File.Delete(file);
            }
        }

        static void HandleParserError(IEnumerable<Error> errs)
        {
            foreach (var err in errs)
                Console.WriteLine(err.ToString());
        }
    }
}

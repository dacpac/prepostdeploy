using System;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace dacpacModifier
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input dacpac to read.")]
        public string InputFile { get; set; }

        [Option("PreDeployFile", HelpText = "The path to the Pre Deploy Script file that will be overridden in the dacpac if one if provided")]
        public string PreDeployFile { get; set; }

        [Option("PostDeployFile", HelpText = "The path to the Post Deploy Script file that will be overridden in the dacpac if one if provided")]
        public string PostDeployFile { get; set; }

        [Option("ChecksumPreDeployFile", HelpText = "Use this bool option to create a Checksum for the predeploy.sql file inside of the dacpac")]
        public bool ChecksumPreDeployFile { get; set; }

        [Option("ChecksumPostDeployFile", HelpText = "Use this bool option to create a Checksum for the postdeploy.sql file inside of the dacpac")]
        public bool ChecksumPostDeployFile { get; set; }

        [Option('v', "verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("dacpacPrePostDeploy", "v 0.3"),
                Copyright = new CopyrightInfo("Craig Ottley-Thistlethwaite", DateTime.Now.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("twitter:@Craig_Ottley");
            help.AddOptions(this);
            return help;
        }
    }
}

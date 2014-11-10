using System;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;
using System.Linq;

namespace dacpacModifier
{
    class PreDeployChecksum
    {
        /// <summary>
        /// Method used to override the post deploy file in a dacpac. 
        /// </summary>
        /// <param name="InputFile"></param>
        /// <param name="args"></param>
        public static void createPreDeployChecksumElement(FileInfo InputFile, Options args)
        {
            Package dacpac = Package.Open(InputFile.FullName, FileMode.OpenOrCreate);
            PackagePartCollection _parts;
            bool _IsPreDeployFile = false;
            try
            {
                _parts = dacpac.GetParts();

                foreach (PackagePart _part in _parts)
                {
                    if (_part.Uri.ToString() == Constants.PreDeployUri)
                    {
                        _IsPreDeployFile = true;
                    }

                }

                Uri preDeployUri = PackUriHelper.CreatePartUri(new Uri(Constants.PreDeployUri, UriKind.Relative));
                PackagePart dacPreDeployPart;

                if (_IsPreDeployFile == true)
                {
                    if (args.Verbose)
                    {
                        Console.WriteLine("Pre Deployment Script Found...");
                    }
                    dacPreDeployPart = dacpac.GetPart(preDeployUri);

                    if (args.ChecksumPreDeployFile)
                    {
                        byte[] byteArray = Checksum.CalculateChecksum(dacPreDeployPart.GetStream());
                        string readableByteArray = string.Concat(byteArray.Select(s => s.ToString("X2")));

                        if (args.Verbose)
                        {
                            Console.WriteLine("Pre Deployment Checksum: {0}", readableByteArray);
                        }
                    }
                }
                else
                {
                    if (args.Verbose)
                    {
                        Console.WriteLine("Pre Deployment Script Not Found inside dacpac: {0}", args.InputFile);
                    }
                }

                dacpac.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

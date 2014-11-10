using System;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;
using System.Linq;

namespace dacpacModifier
{
    class PostDeployChecksum
    {
        /// <summary>
        /// Method used to create a Checksum on the pre deploy file in a dacpac. 
        /// </summary>
        /// <param name="InputFile"></param>
        /// <param name="args"></param>
        public static void createPostDeployChecksumElement(FileInfo InputFile, Options args)
        {
            Package dacpac = Package.Open(InputFile.FullName, FileMode.OpenOrCreate);
            PackagePartCollection _parts;
            bool _IsPostDeployFile = false;
            try
            {
                _parts = dacpac.GetParts();

                foreach (PackagePart _part in _parts)
                {
                    if (_part.Uri.ToString() == Constants.PostDeployUri)
                    {
                        _IsPostDeployFile = true;
                    }

                }

                Uri PostDeployUri = PackUriHelper.CreatePartUri(new Uri(Constants.PostDeployUri, UriKind.Relative));
                PackagePart dacPostDeployPart;

                if (_IsPostDeployFile == true)
                {
                    if (args.Verbose)
                    {
                        Console.WriteLine("Post Deployment Script Found...", args.PostDeployFile);
                    }
                    dacPostDeployPart = dacpac.GetPart(PostDeployUri);

                    if (args.ChecksumPostDeployFile)
                    {
                        byte[] byteArray = Checksum.CalculateChecksum(dacPostDeployPart.GetStream());
                        string readableByteArray = string.Concat(byteArray.Select(s => s.ToString("X2")));

                        if (args.Verbose)
                        {
                            Console.WriteLine("Post Deployment Checksum: {0}", readableByteArray);
                        }
                    }
                }
                else
                {
                    if (args.Verbose)
                    {
                        Console.WriteLine("Post Deployment File Not Found inside dacpac: {0}", args.InputFile);
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
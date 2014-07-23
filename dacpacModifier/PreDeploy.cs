using System;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;

namespace dacpacModifier
{
    class PreDeploy
    {
        /// <summary>
        /// Method used to override the pre deploy file in a dacpac. 
        /// </summary>
        /// <param name="InputFile"></param>
        /// <param name="PreDeployFile"></param>
        /// <param name="args"></param>
        public static void overridePreDeployFile(FileInfo InputFile, string PreDeployFile, Options args)
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
                        Console.WriteLine("Pre Deployment Script Found, Overriding predeploy.sql with contents from: {0}", args.PreDeployFile);
                    }
                    dacPreDeployPart = dacpac.GetPart(preDeployUri);
                }
                else
                {
                    if (args.Verbose)
                    {
                        Console.WriteLine("Pre Deployment Script Not Found inside dacpac, Creating predeploy.sql with contents from: {0}", args.PreDeployFile);
                    }
                    dacPreDeployPart = dacpac.CreatePart(preDeployUri, MediaTypeNames.Text.Plain);
                }

                using (FileStream fileStream = new FileStream(
                        PreDeployFile.ToString(), FileMode.Open, FileAccess.Read))
                {
                    dacPreDeployPart.GetStream().SetLength(0);
                    fileStream.CopyTo(dacPreDeployPart.GetStream());
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

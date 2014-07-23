using System;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;

namespace dacpacModifier
{
    class PostDeploy
    {
        /// <summary>
        /// Method used to override the pre deploy file in a dacpac. 
        /// </summary>
        /// <param name="InputFile"></param>
        /// <param name="PostDeployFile"></param>
        /// <param name="args"></param>
        public static void overridePostDeployFile(FileInfo InputFile, string PostDeployFile, Options args)
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
                        Console.WriteLine("Post Deployment Script Found, Overriding postdeploy.sql with contents from: {0}", args.PostDeployFile);
                    }
                    dacPostDeployPart = dacpac.GetPart(PostDeployUri);
                }
                else
                {
                    if (args.Verbose)
                    {
                        Console.WriteLine("Post Deployment Script Not Found inside dacpac, Creating postdeploy.sql with contents from: {0}", args.PostDeployFile);
                    }
                    dacPostDeployPart = dacpac.CreatePart(PostDeployUri, MediaTypeNames.Text.Plain);
                }

                using (FileStream fileStream = new FileStream(
                        PostDeployFile.ToString(), FileMode.Open, FileAccess.Read))
                {
                    dacPostDeployPart.GetStream().SetLength(0);
                    fileStream.CopyTo(dacPostDeployPart.GetStream());
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
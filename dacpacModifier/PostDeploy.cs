using System;
using System.IO;
using System.IO.Packaging;


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
            Package dacpac = Package.Open(InputFile.FullName, FileMode.Open);
            try
            {
                Uri PostDeployUri = PackUriHelper.CreatePartUri(new Uri(Constants.PostDeployUri, UriKind.Relative));
                PackagePart dacPostDeployPart = dacpac.GetPart(PostDeployUri);

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

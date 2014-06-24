using System;
using System.IO;
using System.IO.Packaging;

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
            Package dacpac = Package.Open(InputFile.FullName, FileMode.Open);
            try
            {
                Uri preDeployUri = PackUriHelper.CreatePartUri(new Uri(Constants.PreDeployUri, UriKind.Relative));
                PackagePart dacPreDeployPart = dacpac.GetPart(preDeployUri);

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

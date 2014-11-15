using System;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;
using System.Linq;
using System.Xml;


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

                        // Add the calculated checksum to the Checksums part in the Origin file.
                        Uri originUri = PackUriHelper.CreatePartUri(new Uri(Constants.OriginXmlUri, UriKind.Relative));
                        PackagePart dacOriginPart = dacpac.GetPart(originUri);
                        XmlDocument dacOriginXml = new XmlDocument();
                        dacOriginXml.Load(XmlReader.Create(dacOriginPart.GetStream()));

                        XmlNamespaceManager xmlns = new XmlNamespaceManager(dacOriginXml.NameTable);
                        xmlns.AddNamespace("dac", Constants.DacOriginXmlns);

                        // Assumption the Checksums Element always exists when the dacpac is built.
                        XmlNode _checksums = dacOriginXml.SelectSingleNode("/dac:DacOrigin/dac:Checksums", xmlns);
                        XmlElement _checksum = dacOriginXml.CreateElement("Checksum", Constants.DacOriginXmlns);

                        _checksum.SetAttribute("Uri", Constants.PreDeployUri);
                        _checksum.InnerText = readableByteArray;
                        // Add the new checksum to the existing Checksums Element
                        _checksums.AppendChild(_checksum);

                        XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings();
                        _xmlWriterSettings.Encoding = System.Text.Encoding.UTF8;
                        _xmlWriterSettings.Indent = true;
                        XmlWriter _xmlWriter = XmlWriter.Create(dacOriginPart.GetStream(FileMode.Open, FileAccess.Write), _xmlWriterSettings);
                                                
                        dacOriginXml.Save(_xmlWriter);
                        
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

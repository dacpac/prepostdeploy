using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace dacpacModifier
{
    class Checks
    {
        /// <summary>
        /// Checks wheather the supplied file is a dacpac
        /// </summary>
        /// <param name="dacpacInputFile"></param>
        /// <param name="args"></param>
        /// <returns>bool</returns>
        public static bool IsDacPac(FileInfo dacpacInputFile, Options args)
        {
            bool flag = false;
            int counter = 0;
            try
            {
                Package dacpac = Package.Open(dacpacInputFile.FullName, FileMode.Open, FileAccess.Read);
                try
                {
                    Uri originUri = PackUriHelper.CreatePartUri(new Uri(Constants.OriginXmlUri, UriKind.Relative));
                    PackagePart dacOriginPart = dacpac.GetPart(originUri);
                    XDocument dacOriginXml = XDocument.Load(XmlReader.Create(dacOriginPart.GetStream()));

                    // Make sure there is a root node
                    if (dacOriginXml.Root != null)
                    {
                        if (args.Verbose)
                            Console.WriteLine("Checking dacpac...");

                        // Check the Namespace Name is as expected
                        if (string.Compare(dacOriginXml.Root.Name.NamespaceName, Constants.DacOriginXmlns, StringComparison.Ordinal) == 0)
                        {
                            counter += 1;
                        }
                        // Check the root node has the correct name
                        if (string.Compare(dacOriginXml.Root.Name.LocalName, Constants.DacOriginRoot, StringComparison.Ordinal) == 0)
                        {
                            counter += 1;
                        }

                        XNamespace xmls = Constants.DacOriginXmlns;
                        var productSchema = from ps in dacOriginXml.Descendants(xmls + "Operation")
                                            select new 
                                            { 
                                                ps = (string) ps.Element(xmls + Constants.ProductSchemaElement) 
                                            };
                        foreach (var e in productSchema)
                        {
                            if (string.Compare(e.ps, Constants.DacOriginXmlns, StringComparison.Ordinal) == 0)
                            {
                                counter += 1;
                            }
                        }

                        var checkSum = dacOriginXml.Root.Descendants(xmls + "Checksums").Elements(xmls + "Checksum")
                                                   .Where(x => x.Attribute("Uri").Value == Constants.ModelXmlUri)
                                                   .FirstOrDefault();

                        if (checkSum != null)
                        {
                            if (args.Verbose)
                            {
                                Console.WriteLine("Checksum Element exists");
                            }
                            counter += 1;
                        }
                    }

                }
                finally
                {
                    if (!(dacpac == null))
                    {
                        dacpac.Close();
                    }
                        
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                flag = false;
            }

            if (counter == 4)
                flag = true;

            return flag;
        }
    }
}

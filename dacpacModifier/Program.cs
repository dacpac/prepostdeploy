using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CommandLine;

namespace dacpacModifier
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = new Options();

                if (Parser.Default.ParseArguments(args, options))
                {
                    Modifier.Modify(options);
                }
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine(fnfe);
            }
            catch (FileFormatException ffe)
            {
                Console.WriteLine(ffe.Message);
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

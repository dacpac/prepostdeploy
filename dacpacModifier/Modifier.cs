﻿using System;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Linq;

namespace dacpacModifier
{
    class Modifier
    {
        /// <summary>
        /// First checks if file exists, then checks the file is a valid dacpac and 
        /// will then go through the dacpac and remove all elements of the specified type
        /// </summary>
        /// <param name="args">The parsed arguments</param>
        public static void Modify(Options args)
        {
            #region Checks
            // Check to see if anything has been supplied for input file
            if (string.IsNullOrEmpty(args.InputFile))
            {
                throw new ApplicationException(Constants.ErrorNoInputFile);
            }
            // Check if the file exists
            FileInfo fi = new FileInfo(args.InputFile);
            if (!fi.Exists)
            {
                throw new FileNotFoundException(args.InputFile);
            }
            // Check the file is a valid dacpac
            if (!Checks.IsDacPac(fi, args))
            {
                throw new FileFormatException(Constants.ErrorNotAValidDacpac);
            }

            if (args.PreDeployFile == null && args.PostDeployFile == null &&
                args.ChecksumPreDeployFile == false && args.ChecksumPostDeployFile == false)
            {
                throw new InvalidOperationException(Constants.ErrorNoPreOrPostDeployFile);
            }
            #endregion

            #region Pre Deploy File Override
            if (args.PreDeployFile != null)
            {
                Console.WriteLine("~ Pre Deploy ~");
                string[] _predeployscript = args.PreDeployFile.Split(';').Distinct().ToArray();
                if (_predeployscript.Count() > 1)
                {
                    throw new InvalidOperationException(Constants.ErrorMoreThanOnePrePostDeployFile);
                }

                Boolean DoesExist = File.Exists(_predeployscript[0]);
                if (!DoesExist)
                {
                    throw new FileNotFoundException(Constants.ErrorFileNotFound);
                }
                else
                {
                    // Override the file
                    PreDeploy.overridePreDeployFile(fi, _predeployscript[0], args);
                }
            }
            #endregion

            #region Checksum Pre Deploy File
            if (args.ChecksumPreDeployFile)
            {
                Console.WriteLine("~ Checksum Pre Deploy ~");
                // Generate Checksum
                PreDeployChecksum.createPreDeployChecksumElement(fi, args);
            }
            #endregion

            #region Post Deploy File Override
            if (args.PostDeployFile != null)
            {
                Console.WriteLine("~ Post Deploy File ~");
                string[] _postdeployscript = args.PostDeployFile.Split(';').Distinct().ToArray();
                if (_postdeployscript.Count() > 1)
                {
                    throw new InvalidOperationException(Constants.ErrorMoreThanOnePrePostDeployFile);
                }

                Boolean DoesExist = File.Exists(_postdeployscript[0]);
                if (!DoesExist)
                {
                    throw new FileNotFoundException(Constants.ErrorFileNotFound);
                }
                else
                {
                    // Override the file
                    PostDeploy.overridePostDeployFile(fi, _postdeployscript[0], args);
                }

            }
            #endregion

            #region Checksum Post Deploy File
            if (args.ChecksumPostDeployFile)
            {
                Console.WriteLine("~ Checksum Post Deploy ~");
                // Generate Checksum
                PostDeployChecksum.createPostDeployChecksumElement(fi, args);
            }
            #endregion
        }

        public static void OutputToConsole(Options args)
        {
            Console.WriteLine("Input File: {0}", args.InputFile);
        }

    }
}


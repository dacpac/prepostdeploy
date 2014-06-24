namespace dacpacModifier
{
    class Constants
    {
        public const string ModelXmlUri = "/model.xml";
        public const string OriginXmlUri = "/origin.xml";
        public const string PreDeployUri = "/predeploy.sql";
        public const string PostDeployUri = "/postdeploy.sql";
        public const string DacOriginXmlns = "http://schemas.microsoft.com/sqlserver/dac/Serialization/2012/02";
        public const string DacOriginRoot = "DacOrigin";
        public const string ProductSchemaElement = "ProductSchema";
        public const string ChecksumElement = "Checksum";
        public const string ErrorInvalidParameter = "Error Invalid Parameter";
        public const string ErrorFileNotFound = "Error File Not Found";
        public const string ErrorNoInputFile = "Error No Input File";
        public const string ErrorNotAValidDacpac = "Error Not A Valid dacpac";
        public const string ErrorMoreThanOnePrePostDeployFile = "Error More Than One Pre/Post Deploy Files Supplied";
        public const string ErrorNoPreOrPostDeployFile = "Error No Pre or Post Deploy File Has Been Supplied";
    }
}
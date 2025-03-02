namespace knowledge_sharing_platform_cloud.Config
{
    /**
     * app settings configuration 
     */
    public class AppSettings
    {
        static IConfiguration Config {  get; set; }
        /**
         * appsetting configuration data
         */
        public AppSettings(IConfiguration config)
        {
            Config = config;
        }

        /**
         * key can be like AWS:S3...
         */
        public static string GetVal(string key)
        {
            if (key == null)
            {
                return string.Empty;
            }
            return Config[key];
        }


    }
}

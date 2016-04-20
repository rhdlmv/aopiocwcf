namespace AOPIOC.Wcf
{
    using System;
    using System.Configuration;

    public class WcfUtils
    {
        public static string GetNameOfDefaultConnectionString()
        {
            ConnectionStringSettings settings = null;
            if (ConfigurationManager.ConnectionStrings.Count == 0)
            {
                return null;
            }
            for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                if (!"LocalSqlServer".Equals(ConfigurationManager.ConnectionStrings[i].Name))
                {
                    settings = ConfigurationManager.ConnectionStrings[i];
                    break;
                }
            }
            if (settings == null)
            {
                return null;
            }
            return settings.Name;
        }
    }
}


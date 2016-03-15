using System.Configuration;

namespace BusinessRules.Core
{
    public class ConfigurationManager : ConfigurationSection
    {
        public static ConfigurationManager Configuration = null;

        public static void Configure()
        {
            Configuration =
                (ConfigurationManager)System.Configuration.ConfigurationManager.GetSection("BusinessRulesGroup/BusinessRules");
        }

        #region properties

        [ConfigurationProperty("constantsPath", DefaultValue = "constants.xml", IsRequired = true)]
        public string ConstantsPath
        {
            get
            {
                return (string)this["constantsPath"];
            }
            set
            {
                this["constantsPath"] = value;
            }
        }

        [ConfigurationProperty("rulesPath", DefaultValue = "rules.xml", IsRequired = true)]
        public string RulesPath
        {
            get
            {
                return (string)this["rulesPath"];
            }
            set
            {
                this["rulesPath"] = value;
            }
        }

        [ConfigurationProperty("entitiesPath", DefaultValue = "entities.xml", IsRequired = true)]
        public string EntitiesPath
        {
            get
            {
                return (string)this["entitiesPath"];
            }
            set
            {
                this["entitiesPath"] = value;
            }
        }

        [ConfigurationProperty("basicMethodsPath", DefaultValue = "basicMethodsPath.xml", IsRequired = true)]
        public string BasicMethodsPath
        {
            get
            {
                return (string)this["basicMethodsPath"];
            }
            set
            {
                this["basicMethodsPath"] = value;
            }
        }
        #endregion
    }
}

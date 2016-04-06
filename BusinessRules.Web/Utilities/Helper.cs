using System.Collections.Generic;

namespace BusinessRules.Web
{
    public class Helper
    {
        public static List<string> AvailableFacts
        {
            get
            {
                return Core.Parameters.AvialableFacts();
            }
        }

        public static List<string> AvailableRules
        {
            get
            {
                return Core.Parameters.AvialableRules();
            }
        }

        public static List<string> AvailableConstants
        {
            get
            {
                return Core.Parameters.AvialableConstants();
            }
        }
    }
}
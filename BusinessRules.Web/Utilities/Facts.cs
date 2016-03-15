using System.Collections.Generic;

namespace BusinessRules.Web
{
    public class FactsHelper
    {
        public static List<string> AvailableFacts
        {
            get
            {
                return Core.Parameters.AvialableFacts();
            }
        }
    }
}
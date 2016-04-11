using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace BusinessRules.Client
{
    public class BusinessRulesClient
    {
        #region Fields
        private string _url = string.Empty;
        private const string _executeRuleRequestUri = "api/async/executerule";
        private const string _executeRuleGroupRequestUri = "api/async/executerulegroup";
        #endregion

        #region .ctor
        public BusinessRulesClient()
        {
        }
        public BusinessRulesClient(string url)
        {
            _url = url;
        }
        #endregion

        #region Properties
        public string Url
        {
            get
            {
                return _url;
            }

            set
            {
                _url = value;
            }
        }
        #endregion

        #region public methods
        public T ExecuteRule<T>(string ruleName, T fact)
        {
            return Execute(ruleName, fact, _executeRuleRequestUri);
        }

        public T ExecuteRuleGroup<T>(string ruleGroupName, T fact)
        {
            return Execute(ruleGroupName, fact, _executeRuleGroupRequestUri);
        }
        #endregion

        #region private methods
        private T Execute<T>(string ruleName, T fact, string requestUri)
        {
            ExecuteRuleDefinition executeRule = new ExecuteRuleDefinition()
            {
                entity = fact,
                ruleName = ruleName
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_url);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(executeRule), Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode();
            var responseBytes = response.Content.ReadAsByteArrayAsync().Result;
            var responseJson = System.Text.Encoding.UTF8.GetString(responseBytes);
            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        #endregion

    }
}

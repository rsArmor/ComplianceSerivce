using System;
using ComplianceService.Models.Fedsfm;
using ComplianceService.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace ComplianceService.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class FedsfmCheckService
    {
        private readonly ILogger _logger;
        private readonly ConnectionSettings _connectionSettings;
        public FedsfmCheckService(ILogger<FedsfmCheckService> logger, IOptions<ConnectionSettings> connectionSettings)
        {
            _logger = logger;
            _connectionSettings = connectionSettings.Value;
        }
        /// <summary>
        /// Проверка ЮЛ в списке террористов
        /// * TerroristException если будет найден
        /// </summary>
        public TerroristResponse CheckOrganizationByInn(string inn)
        {
            return RequestTerrorists(inn);
        }
        /// <summary>
        /// Проверка ФЛ в списке террористов
        /// * TerroristException если будет найден
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="lastname"></param>
        public TerroristResponse CheckIndividualContractor(string firstName, string middleName, string lastname)
        {
            var query = $"{lastname} {firstName} {middleName} ";
            return RequestTerrorists(query);
        }
        /// <summary>
        /// Получение ответа от сервиса Росфинмониторинг
        /// </summary>
        private TerroristResponse RequestTerrorists(string query)
        {
            _logger.LogDebug($"{DateTime.Now.ToLongTimeString()} Request:\r\n {query}");
            var restClient = new RestClient(_connectionSettings.FedsfmUrl);
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddParameter("query", query);
            var restResponse = restClient.Execute<TerroristResponse>(restRequest);
            _logger.LogDebug($"{DateTime.Now.ToLongTimeString()} Response:\r\n{JsonConvert.SerializeObject(restResponse.Data)}");
            return restResponse.Data;       
        }
    }
}

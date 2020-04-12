using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using RestSharp.Serializers;
using RestSharp.Deserializers;
using ComplianceService.Models.BankApi;
using RestSharp;
using Newtonsoft.Json;
using ComplianceService.Models;
using ComplianceService.Models.Fedsfm;
using System.Reflection;
using System.Security.Cryptography;
using ComplianceService.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ComplianceService.Services
{
    /// <summary>
    /// Проверка контрагента через Единый веб-сервис проверок Альфа-Банка 
    /// </summary>
    public class BankCheckService

    {
        private readonly Dictionary<string, string> _checkMap = new Dictionary<string, string>
        {
            { "S01", "Санкционный список" },
            { "N04", "Репутационные риски" }
        };
        private const string BankCheckSource = "Альфа-Банк";
        private const string WSCustomerComplianceCheckGet = "WSCustomerComplianceCheckGet";
        private const string WSCustomerTerrorism = "WSCustomerTerrorism";
        private const string WSCodeInvalidIdentityCard = "WSCodeInvalidIdentityCard";
        private readonly BankApiJsonSerializer _serializer = new BankApiJsonSerializer();
        private readonly FedsfmCheckService _fedsfmService;
        private readonly ConnectionSettings _connectionSettings;
        private readonly ILogger _logger;
        private readonly CertificateSettings _certificatePassword;
        public BankCheckService(FedsfmCheckService fedsfmService, IOptions<ConnectionSettings> connectionSettings, ILogger<BankCheckService> logger, IOptions<CertificateSettings> certificateSettings)
        {
            _fedsfmService = fedsfmService;
            _connectionSettings = connectionSettings.Value;
            _logger = logger;
            _certificatePassword = certificateSettings.Value;
        }
        /// <summary>
        /// Осуществить проверку Физ лица
        /// </summary>
        /// <param name="individualParams"></param>
        /// <returns></returns>
        public List<ContractorCheckResult> CheckIndividual(IndividualParams individualParams)
        {
            var checkResults = new List<ContractorCheckResult>();
            checkResults.AddRange(CheckInBankService(individualParams.Inn, individualParams.DateBirthDay, individualParams.ExternalSystemCode,
                individualParams.LastName, individualParams.FirstName, individualParams.MiddleName, "FL"));
            checkResults.Add(CheckInFedsfm(individualParams.FirstName, individualParams.MiddleName, individualParams.LastName));
            return checkResults;
        }
        /// <summary>
        /// Осуществить проверку организации по ИНН
        /// </summary>
        /// <param name="orgParams"></param>
        /// <returns></returns>
        public List<ContractorCheckResult> CheckOrganizationByInn(BaseParams orgParams)
        {
            var checkResults = new List<ContractorCheckResult>();
            checkResults.AddRange(CheckInBankService(orgParams.ExternalSystemCode, orgParams.Inn, "UL"));
            checkResults.Add(CheckInFedsfm(orgParams.Inn));
            return checkResults;
        }
        /// <summary>
        /// Осуществить проверку ИП
        /// </summary>
        /// <param name="managementName"></param>
        /// <param name="inn"></param>
        /// <param name="customerType"></param>
        /// <param name="terroristCheckInfo"></param>
        public List<ContractorCheckResult> CheckEntrepreneur(IndividualParams entrepeneurParams)
        {
            var checkResults = new List<ContractorCheckResult>();
            checkResults.AddRange(CheckInBankService(entrepeneurParams.Inn, entrepeneurParams.DateBirthDay, entrepeneurParams.ExternalSystemCode,
                entrepeneurParams.LastName, entrepeneurParams.FirstName, entrepeneurParams.MiddleName, "IP"));           
            checkResults.Add(CheckInFedsfm(entrepeneurParams.FirstName, entrepeneurParams.MiddleName, entrepeneurParams.LastName));
            return checkResults;
        }
        /// <summary>
        /// Отправить запрос в API Банка 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private BankCheckResponse RequestApiServiceBank(object data)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var certStream = Assembly.GetEntryAssembly().GetManifestResourceStream("ComplianceService.certificate.pfx");
            var certData = GetByteArray(certStream);
            X509Certificate2 certificate = null;
            try
            {
                certificate = new X509Certificate2(certData, _certificatePassword.CertificatePassword);
            }
            catch (CryptographicException ex)
            {
                throw new Exception(ex.Message);
            }
            _logger.LogDebug($"{DateTime.Now.ToLongTimeString()} Request:\r\n{JsonConvert.SerializeObject(data)}");
            var restClient = new RestClient(_connectionSettings.BankApiUrl); 
            restClient.ClientCertificates = new X509CertificateCollection() { certificate };
            var restRequest = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };
            restRequest.AddHeader("content-type", "application/json");
            restRequest.AddHeader("accept", "application/json");
            restRequest.AddHeader("X-IBM-Client-Id", _connectionSettings.ClientId);
            restRequest.JsonSerializer = _serializer;
            restRequest.AddJsonBody(data);
            var restResponse = restClient.Execute<BankCheckResponse>(restRequest);
            _logger.LogDebug($"{DateTime.Now.ToLongTimeString()} Response:\r\n{restResponse.Content}");
            return restResponse.Data;
        }
        private List<ContractorCheckResult> PrepareResult(string name, BankCheckResponse response)
        {
            var checkResults = new List<ContractorCheckResult>();
            if (response == null)
            {
                return null;
            }
            var checks = response.OutParms.Checks;
            var terroristCheck = checks.FirstOrDefault(c => c.CheckName == WSCustomerTerrorism);
            if (terroristCheck != null)
            {
                var checkResult = new ContractorCheckResult();
                checkResult.CheckName = "Терроризм";
                checkResult.CheckSource = BankCheckSource;
                checkResult.ContractorName = name;
                checkResult.IsSucces = !CheckOut.FailedChecks.Contains(terroristCheck.CheckResult);
                checkResult.RiskCodes = null;
                checkResult.IsNeedHandCheck = CheckOut.FailedChecks.Contains(terroristCheck.CheckResult);
                checkResult.Description = CheckOut.FailedChecks.Contains(terroristCheck.CheckResult) ? "Проверка не выполнялась" : terroristCheck.Comment?.Trim();
                checkResults.Add(checkResult);
            }
            if (response.OutParms.OutParmsWSComplianceCheck?.ResultSet1 == null)
            {
                return checkResults;
            }
            foreach (var resultSet in response.OutParms.OutParmsWSComplianceCheck.ResultSet1)
            {
                var checkResult = new ContractorCheckResult();
                checkResult.CheckName = _checkMap[resultSet.Idp];
                checkResult.CheckSource = BankCheckSource;
                checkResult.ContractorName = name;
                checkResult.IsNeedHandCheck = resultSet.Chk == CheckOut.SuccessfullCheck;
                checkResult.IsSucces = resultSet.Chk != CheckOut.SuccessfullCheck;
                checkResult.RiskCodes = resultSet.Atr.RSK != null && resultSet.Atr.RSK.Count > 0
                        ? string.Join(", ", resultSet.Atr.RSK)
                        : null;
                checkResult.Description = resultSet.Nms;
                checkResults.Add(checkResult);
            }
            return checkResults;
        }
        /// <summary>
        /// Проверить Паспорт в Сервисе Банка
        /// </summary>
        /// <param name="identityCardParams"></param>
        /// <returns></returns>
        public ContractorCheckResult CheckIdentityCard(IdentityCard identityCardParams)
        {            
            var response = RequestApiServiceBank(new
            {
                exs = identityCardParams.ExternalSystemCode,
                checks = new List<CheckIn>()
                {
                    new CheckIn(WSCodeInvalidIdentityCard),
                },
                fnm1 = identityCardParams.LastName,
                fnm2 = identityCardParams.FirstName,
                fnm3 = identityCardParams.MiddleName,
                dtbr = identityCardParams.DateBirthDay?.ToString("yyyy-MM-dd"),
                crf = string.IsNullOrWhiteSpace(identityCardParams.Inn) ? null : identityCardParams.Inn,
                ucd = identityCardParams.TypeIdentityDocument,
                ser = identityCardParams.SerialIdentityCard,
                num = identityCardParams.NumberIedentityCard,
                dopn = identityCardParams.DateOfIssue?.ToString("yyyy-MM-dd"),
                customerType = "FL"
            });
            var checkResult = new ContractorCheckResult();
            var checks = response.OutParms.Checks;
            var identityCardCheck = checks.FirstOrDefault(c => c.CheckName == WSCodeInvalidIdentityCard);
            if (identityCardCheck != null)
            {
                checkResult.CheckName = "Проверка в сервисе недействительных паспортов";
                checkResult.CheckSource = BankCheckSource;
                checkResult.ContractorName = $"{identityCardParams.LastName} {identityCardParams.FirstName} {identityCardParams.MiddleName}";
                checkResult.IsSucces = !CheckOut.FailedChecks.Contains(identityCardCheck.CheckResult);
                checkResult.RiskCodes = null;
                checkResult.IsNeedHandCheck = CheckOut.FailedChecks.Contains(identityCardCheck.CheckResult);
                checkResult.Description = CheckOut.FailedChecks.Contains(identityCardCheck.CheckResult) ? "Проверка неуспешна" : "Клиент имеет действительный паспорт";
            }
            return checkResult;
        }
        /// <summary>
        /// Проверить в сервисе Альфа-Банка Физ лицо или ИП
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="dateBirthDay"></param>
        /// <param name="externalSystemCode"></param>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="custType"></param>
        /// <returns></returns>
        private List<ContractorCheckResult> CheckInBankService(string inn, DateTime? dateBirthDay,
            string externalSystemCode, string lastName,string firstName,string middleName,string custType)
        {
            if (string.IsNullOrWhiteSpace(inn) && dateBirthDay == null)
            {
                return null;
            }
            var response = RequestApiServiceBank(new
            {
                exs = externalSystemCode,
                checks = new List<CheckIn>()
                {
                    new CheckIn(WSCustomerComplianceCheckGet),
                    new CheckIn(WSCustomerTerrorism)
                },
                fnm1 = lastName,
                fnm2 = firstName,
                fnm3 = middleName,
                dtbr = dateBirthDay?.ToString("yyyy-MM-dd"),
                crf = string.IsNullOrWhiteSpace(inn) ? null : inn,
                chklst = "N04,S01",
                customerType = custType
            });
            return PrepareResult($"{lastName} {firstName} {middleName}", response);
        }
        /// <summary>
        /// Проверить в сервисе Альфа-Банка Юр лицо
        /// </summary>
        /// <param name="externalSystemCode"></param>
        /// <param name="inn"></param>
        /// <param name="custType"></param>
        /// <returns></returns>
        private List<ContractorCheckResult> CheckInBankService(string externalSystemCode, string inn,string custType)
        {
            var result = RequestApiServiceBank(new
            {
                exs = externalSystemCode,
                checks = new List<CheckIn>()
                {
                    new CheckIn(WSCustomerComplianceCheckGet),
                    new CheckIn(WSCustomerTerrorism)
                },
                crf = inn,
                chklst = "S01",
                customerType = custType
            });
            return PrepareResult(inn, result);
        }
        /// <summary>
        /// Проверка Юр лица в сервисе РОСФИНМОНИТОРИНГА
        /// </summary>
        /// <returns></returns>
        private ContractorCheckResult CheckInFedsfm(string inn)
        {
            return PrapareResponseFromFedsfm(inn, _fedsfmService.CheckOrganizationByInn(inn));
        }
        /// <summary>
        /// Проверка Физ лица или ИП в сервисе РОСФИНМОНИТОРИНГА
        /// </summary>
        /// <returns></returns>
        private ContractorCheckResult CheckInFedsfm(string firstName, string middleName, string lastname)
        {
            return PrapareResponseFromFedsfm( $"{lastname} {firstName} {middleName}",
                _fedsfmService.CheckIndividualContractor(firstName,  middleName, lastname));
        }
        private ContractorCheckResult PrapareResponseFromFedsfm(string name, TerroristResponse fedsfmResponse)
        {
            if (fedsfmResponse == null) return null;
            var check = new ContractorCheckResult();
            check.CheckName = "Терроризм";
            check.Description = fedsfmResponse.Suggestions?.FirstOrDefault();
            check.RiskCodes = null;
            check.ContractorName = name;
            check.IsSucces = fedsfmResponse.Suggestions == null || !fedsfmResponse.Suggestions.Any();
            check.CheckSource = "Росфинмониторинг";
            check.IsNeedHandCheck =  fedsfmResponse.Suggestions.Any();
            return check;
        } 
        /// <summary>
        /// Получить массив байтов
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] GetByteArray(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public class BankApiJsonSerializer : RestSharp.Serializers.ISerializer, IDeserializer
        {
            public BankApiJsonSerializer()
            {
                ContentType = "application/json";
            }

            public string Serialize(object obj)
            {
                return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });
            }

            public T Deserialize<T>(IRestResponse response)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }

            public string RootElement { get; set; }

            public string Namespace { get; set; }

            public string DateFormat { get; set; }

            public string ContentType { get; set; }
        }
    }
}

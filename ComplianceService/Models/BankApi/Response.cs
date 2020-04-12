using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComplianceService.Models.BankApi
{
    public class BankCheckResponse
    {
        [JsonProperty("outParms")]
        public OutParms OutParms { get; set; }

        public BankCheckResponse()
        {
            OutParms = new OutParms();
        }
    }

    public class OutParms
    {
        public OutParms()
        {
            Checks = new List<CheckOut>();
        }

        [JsonProperty("checks")]
        public List<CheckOut> Checks { get; set; }

        [JsonProperty("outParmsWSComplianceCheck")]
        public OutParmsWsComplianceCheck OutParmsWSComplianceCheck { get; set; }
    }

    public class CheckOut
    {
        public const string CheckNotPerformed = "E";
        public const string SuccessfullCheck = "N";
        public const string FailedCheck = "Y";
        public const string MissingParametersCode = "PBS0100";

        public static string[] FailedChecks = new string[] { CheckNotPerformed, FailedCheck };
        /// <summary>
        /// Название проверки
        /// </summary>
        [JsonProperty("checkName")]
        public string CheckName { get; set; }

        /// <summary>
        /// Флаг необходимости осуществления проверки
        /// </summary>
        [JsonProperty("isToCall")]
        public string IsToCall { get; set; }

        /// <summary>
        /// Результат проверки
        /// </summary>
        [JsonProperty("checkResult")]
        public string CheckResult { get; set; }

        /// <summary>
        /// Cовпавшие теги
        /// </summary>
        [JsonProperty("matched")]
        public Matched Matched { get; set; }

        /// <summary>
        /// Комментарий к проверке
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        public string idg { get; set; }
    }

    public class Matched
    {
        /// <summary>
        /// Имя совпавшего тега
        /// </summary>
        [JsonProperty("tagName")]
        public string TagName { get; set; }
    }

    public class OutParmsWsComplianceCheck
    {
        [JsonProperty("fres")]
        public string Fres { get; set; }

        [JsonProperty("ResultSet1")]
        public List<ResultSet1> ResultSet1 { get; set; }

        [JsonProperty("ResultSet2")]
        public List<ResultSet2> ResultSet2 { get; set; }

        [JsonProperty("ResultSet3")]
        public List<ResultSet3> ResultSet3 { get; set; }
    }

    public class ResultSet1
    {
        /// <summary>
        /// Код проверки
        /// </summary>
        [JsonProperty("idp")]
        public string Idp { get; set; }

        /// <summary>
        /// Код результата
        /// </summary>
        [JsonProperty("idr")]
        public string Idr { get; set; }

        /// <summary>
        /// Описание результата
        /// </summary>
        [JsonProperty("nms")]
        public string Nms { get; set; }

        /// <summary>
        /// Результат отдельной проверки
        /// </summary>
        [JsonProperty("chk")]
        public string Chk { get; set; }

        /// <summary>
        /// Атрибуты проверок
        /// </summary>
        [JsonProperty("atr")]
        public Atr Atr { get; set; }
    }

    public class Atr
    {
        public Atr()
        {
            RSK = new List<string>();
        }
        public List<string> RSK { get; set; }
    }

    public class ResultSet2
    {
        /// <summary>
        /// ФИО
        /// </summary>
        [JsonProperty("fnm")]
        public string Fnm { get; set; }

        /// <summary>
        /// Дата рождение
        /// </summary>
        [JsonProperty("dtbr")]
        public DateTimeOffset Dtbr { get; set; }

        /// <summary>
        /// Вид документа
        /// </summary>
        [JsonProperty("ucd")]
        public string Ucd { get; set; }

        /// <summary>
        /// Серия документа
        /// </summary>
        [JsonProperty("ser")]
        public string Ser { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        [JsonProperty("num")]
        public long Num { get; set; }

        /// <summary>
        /// Кем выдан документ
        /// </summary>
        [JsonProperty("org")]
        public string Org { get; set; }

        /// <summary>
        /// Дата выдачи документа
        /// </summary>
        [JsonProperty("opn")]
        public DateTimeOffset Opn { get; set; }

        /// <summary>
        /// СНИЛС
        /// </summary>
        [JsonProperty("fior")]
        public string Fior { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        [JsonProperty("crf")]
        public string Crf { get; set; }

        /// <summary>
        /// Код запрета
        /// </summary>
        [JsonProperty("ban")]
        public string Ban { get; set; }
    }

    public class ResultSet3
    {
        /// <summary>
        /// ФИО
        /// </summary>
        [JsonProperty("fnm")]
        public string Fnm { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        [JsonProperty("dtbr")]
        public DateTimeOffset Dtbr { get; set; }

        /// <summary>
        /// Вид документа
        /// </summary>
        [JsonProperty("ucd")]
        public string Ucd { get; set; }

        /// <summary>
        /// Серия документа
        /// </summary>
        [JsonProperty("ser")]
        public string Ser { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        [JsonProperty("num")]
        public long Num { get; set; }

        /// <summary>
        /// Кем выдан документ
        /// </summary>
        [JsonProperty("org")]
        public string Org { get; set; }

        /// <summary>
        /// Адрес регистрации
        /// </summary>
        [JsonProperty("amr")]
        public string Amr { get; set; }

        /// <summary>
        /// Место рождения
        /// </summary>
        [JsonProperty("mr")]
        public string Mr { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        [JsonProperty("crf")]
        public string Crf { get; set; }

        /// <summary>
        /// Признак террориста
        /// </summary>
        [JsonProperty("ter")]
        public long Ter { get; set; }
    }
}

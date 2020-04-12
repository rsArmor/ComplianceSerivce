using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceService.Models.BankApi
{
    public class CheckIn
    {
        /// <summary>
        /// Название проверки
        /// </summary>
        /// <param name="name"></param>
        public CheckIn(string name)
        {
            checkName = name;
            isToCall = "true";
        }
        /// <summary>
        /// Название проверки
        /// </summary>
        public string checkName { get; set; }
        /// <summary>
        /// Флаг необходимости осуществления проверки  ["true", "false", "0", "1"]
        /// </summary>
        public string isToCall { get; set; }
    }
}

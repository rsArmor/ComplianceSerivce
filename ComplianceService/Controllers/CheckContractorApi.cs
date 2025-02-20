/*
 * Сервис проверки контрагентов с целью ПОД/ФТ
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1.0.2-oas3
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ComplianceService.Attributes;
using ComplianceService.Models;
using ComplianceService.Services;
using Microsoft.Extensions.Logging;

namespace ComplianceService.Controllers
{
    [ApiController]
    public class CheckContractorApiController : ControllerBase
    {
        readonly BankCheckService _bankCheckService;
        ILogger<CheckContractorApiController> _logger;
        public CheckContractorApiController(BankCheckService bankCheckService, ILogger<CheckContractorApiController> logger)
        {
            _bankCheckService = bankCheckService;
            _logger = logger;
        }
        /// <summary>
        /// Получить информацию о проверке ИП в сервисе Альфа-Банка
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Информация о проверке</response>
        /// <response code="400">bad input parameter</response>
        [HttpPost]
        [Route("/CheckEntrepreneur/")]
        [ValidateModelState]
        [SwaggerOperation("CheckEntrepreneur")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ContractorCheckResult>), description: "Информация о проверке")]
        public virtual IActionResult CheckEntrepreneur( [FromBody]IndividualParams body)
        {
            if (body == null)
                return this.BadRequest();
            var result = _bankCheckService.CheckEntrepreneur(body);
            return StatusCode(200, new ObjectResult(result));
        }
        /// <summary>
        /// Получить информацию о проверке Физ. лица в сервисе Альфа-Банка
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Информация о проверке</response>
        /// <response code="400">bad input parameter</response>
        [HttpPost]
        [Route("/CheckIndividual/")]
        [ValidateModelState]
        [SwaggerOperation("CheckIndividual")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ContractorCheckResult>), description: "Информация о проверке")]
        public virtual IActionResult CheckIndividual([FromBody] IndividualParams body)
        {
            if (body == null)
                    return this.BadRequest(); 
                var result = _bankCheckService.CheckIndividual(body);
            return StatusCode(200, result);
        }
        /// <summary>
        /// Получить информацию о проверке Юридического лица в сервисе Альфа-Банка
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Информация о проверке</response>
        /// <response code="400">bad input parameter</response>
        [HttpPost]
        [Route("/CheckOrganization/")]
        [ValidateModelState]
        [SwaggerOperation("ChekOrganization")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ContractorCheckResult>), description: "Информация о проверке")]
        public virtual IActionResult ChekOrganization([FromBody]BaseParams body)
        {
            if (body == null)
                return this.BadRequest();
            var result = _bankCheckService.CheckOrganizationByInn(body);
            return StatusCode(200, result);
        }
    }
}

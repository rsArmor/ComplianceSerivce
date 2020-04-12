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
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ComplianceService.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class BaseParams : IEquatable<BaseParams>
    { 
        /// <summary>
        /// Код внешней системы
        /// </summary>
        /// <value>Код внешней системы</value>
        [DataMember(Name="externalSystemCode")]
        public string ExternalSystemCode { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        /// <value>ИНН</value>
        [DataMember(Name="inn")]
        public string Inn { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class BaseParams {\n");
            sb.Append("  ExternalSystemCode: ").Append(ExternalSystemCode).Append("\n");
            sb.Append("  Inn: ").Append(Inn).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((BaseParams)obj);
        }

        /// <summary>
        /// Returns true if BaseParams instances are equal
        /// </summary>
        /// <param name="other">Instance of BaseParams to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(BaseParams other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    ExternalSystemCode == other.ExternalSystemCode ||
                    ExternalSystemCode != null &&
                    ExternalSystemCode.Equals(other.ExternalSystemCode)
                ) && 
                (
                    Inn == other.Inn ||
                    Inn != null &&
                    Inn.Equals(other.Inn)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (ExternalSystemCode != null)
                    hashCode = hashCode * 59 + ExternalSystemCode.GetHashCode();
                    if (Inn != null)
                    hashCode = hashCode * 59 + Inn.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(BaseParams left, BaseParams right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseParams left, BaseParams right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}

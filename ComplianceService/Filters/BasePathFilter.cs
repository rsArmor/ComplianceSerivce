using System.Linq;
using System.Text.RegularExpressions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;
using IDocumentFilter = Swashbuckle.Swagger.IDocumentFilter;

namespace ComplianceService.Filters
{
    /// <summary>
    /// BasePath Document Filter sets BasePath property of Swagger and removes it from the individual URL paths
    /// </summary>
    public class BasePathFilter : Microsoft.OpenApi.Models.OpenApiDocument
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath">BasePath to remove from Operations</param>
        public BasePathFilter(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// Gets the BasePath of the Swagger Doc
        /// </summary>
        /// <returns>The BasePath of the Swagger Doc</returns>
        public string BasePath { get; }

        /// <summary>
        /// Apply the filter
        /// </summary>
        /// <param name="swaggerDoc">SwaggerDocument</param>
        /// <param name="context">FilterContext</param>
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.basePath = this.BasePath;

            var pathsToModify = swaggerDoc.paths.Where(p => p.Key.StartsWith(this.BasePath)).ToList();

            foreach (var path in pathsToModify)
            {
                if (path.Key.StartsWith(this.BasePath))
                {
                    string newKey = Regex.Replace(path.Key, $"^{this.BasePath}", string.Empty);
                    swaggerDoc.paths.Remove(path.Key);
                    swaggerDoc.paths.Add(newKey, path.Value);
                }
            }
        }
    }
}

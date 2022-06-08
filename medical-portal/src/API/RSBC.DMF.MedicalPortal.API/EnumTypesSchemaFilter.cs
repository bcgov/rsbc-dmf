using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace RSBC.DMF.MedicalPortal.API
{

    public class EnumTypesSchemaFilter : ISchemaFilter
    {
        private readonly XDocument _xmlComments;

        public EnumTypesSchemaFilter(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                _xmlComments = XDocument.Load(xmlPath);
            }
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (_xmlComments == null) return;

            if (schema.Enum != null && schema.Enum.Count > 0 &&
                context.Type != null && context.Type.IsEnum)
            {
                schema.Description += "<p>Members:</p><ul>";

                var fullTypeName = context.Type.FullName;

                var fields = context.Type.GetFields(BindingFlags.Public | BindingFlags.Static);


                foreach (var field in fields)
                {
                    string constantName = field.Name;
                    var cv = field.GetRawConstantValue();


                    int constantValue = cv == null ? 0 : (int)cv;
                    
                    var fullEnumMemberName = $"F:{fullTypeName}.{constantName}";

                    var enumMemberComments = _xmlComments.Descendants("member")
                        .FirstOrDefault(m => m.Attribute("name").Value.Equals
                        (fullEnumMemberName, StringComparison.OrdinalIgnoreCase));

                    string description = string.Empty;

                    if (enumMemberComments != null)
                    {
                        var summary = enumMemberComments.Descendants("summary").FirstOrDefault();

                        if (summary != null) 
                        { 
                            description = summary.Value.Trim();
                        }
                    }

                    schema.Description += $"<li>{constantValue}: <i>{constantName}</i>{description}</ li >\n";
                    
                }

                schema.Description += "</ul>";
            }
        }
    }
}

using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.OData;

namespace Rsbc.Dmf.CaseManagement.Models
{
    // Sparse Incident class with Odata annotations
    [global::Microsoft.OData.Client.EntitySet("incidents")]

    [global::Microsoft.OData.Client.Key("incidentid")]
    //[global::Microsoft.OData.Client.EntityType("Microsoft.Dynamics.CRM.incident")]
    public class incident : global::Microsoft.OData.Client.BaseEntityType
    {
        public Guid? incidentid { get; set; }
        public bool dfp_resolvecase { get; set; }
    }
}

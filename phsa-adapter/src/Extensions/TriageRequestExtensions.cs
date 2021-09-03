using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Pssg.Rsbc.Dmf.DocumentTriage;

namespace Rsbc.Dmf.PhsaAdapter.Extensions  
{
    public static class TriageRequestExtensions
    {
        public static void AddItems(this TriageRequest triageRequest, List<QuestionnaireResponse.ItemComponent> items)
        {
            foreach (QuestionnaireResponse.ItemComponent item in items)
            {
                if (item.Item != null && item.Item.Count > 0)
                {
                    // recursive call.
                    triageRequest.AddItems(item.Item);
                }
                else
                {
                    if (item.Answer.Count == 1) // simple response.
                    {
                        QuestionItem qi = new QuestionItem()
                        {
                            Question = item.Text,
                        };

                        var value = item.Answer[0].Value.ToTypedElement();

                        // only add flags that have text associated with them - avoids blank flags being added.
                        if (item.LinkId.ToLower().StartsWith("flag") && !string.IsNullOrEmpty(item.Text))
                        {
                            // add a flag.
                            FlagItem fi = new FlagItem()
                            {
                                Identifier = item.LinkId,
                                Question = item.Text,
                                Result = (bool)value.Value
                            };
                            triageRequest.Flags.Add(fi);
                        }

                        switch (item.Answer[0].TypeName)
                        {
                            case "valueBoolean":
                                qi.Response = $"{(bool)value.Value}";
                                break;
                            case "valueString":
                                qi.Response = $"{(string)value.Value}";
                                break;
                        }
                        triageRequest.Questions.Add(qi);
                    }
                    
                }
            }
                

            
        }
    }
}

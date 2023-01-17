using System;
using System.Collections.Generic;

namespace Pssg.Interfaces.IcbcModels
{

    public class Medicals
    {
       public DateTime IssueDate { get; set; }
       public int IssuingOffice { get; set; }
       public string IssuingOfficeDescription { get; set; }
       public string PhysiciansGuide1 { get; set; }
       public string PhysiciansGuide2 { get; set; }
       public DateTime ExamDate { get; set; }
       public string MedicalDisposition { get; set; }
       public string DispositionDescription { get; set; }

    }
}


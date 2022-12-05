using System;
using System.Collections.Generic;

namespace Rsbc.Dmf.IcbcAdapter.IcbcModels
{
    public class Medicals
    {
        public List<MedicalDetails> MedicalDetails { get; set; }
    }

    public class MedicalDetails
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


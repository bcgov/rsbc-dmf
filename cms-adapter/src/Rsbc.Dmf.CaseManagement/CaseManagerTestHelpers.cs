using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Microsoft.OData.UriParser;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Rsbc.Dmf.CaseManagement
{


    internal partial class CaseManager : ICaseManager
    {

        public Guid? AddDriver(string name, string dl)
        {
            dynamicsContext.SaveChanges();
            var account = dynamicsContext.accounts.ToList().First();

            var person = new contact { lastname = name, fullname = name, parentcustomerid_account = account };

            dynamicsContext.AddTocontacts(person);

            var saveResult = dynamicsContext.SaveChanges();
            var personId = GetCreatedId(saveResult);

            if (person.contactid == null)
            {
                person.contactid = personId;
            }


            var driver = new dfp_driver { dfp_licensenumber = dl, dfp_fullname = name };

            dynamicsContext.AddTodfp_drivers(driver);

            var driverSaveResult = dynamicsContext.SaveChanges();

            var driverId = GetCreatedId(driverSaveResult);

            dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), person);
            dynamicsContext.SaveChanges();

            dynamicsContext.Detach(person);
            dynamicsContext.Detach(driver);

            return driverId;

        }


        public Guid? AddCase(Guid driverId, int caseSequence)
        {

            var driver = dynamicsContext.dfp_drivers.ByKey(driverId).GetValue();
            dynamicsContext.LoadProperty(driver, nameof(dfp_driver.dfp_PersonId));
            var contact = driver.dfp_PersonId;

            var newCase = new incident
            {
                //customerid_contact = contact,
                dfp_dfcmscasesequencenumber = caseSequence
            };


            dynamicsContext.AddToincidents(newCase);
            dynamicsContext.SetLink(newCase, nameof(incident.customerid_contact), contact);

            var caseSaveResult = dynamicsContext.SaveChanges();

            var caseId = GetCreatedId(caseSaveResult);

            dynamicsContext.SetLink(newCase, nameof(incident.dfp_DriverId), driver);

            dynamicsContext.SaveChanges();

            dynamicsContext.Detach(newCase);
            dynamicsContext.Detach(driver);

            return caseId;

        }

        public void DeleteDriver(Guid driverId)
        {
            try
            {
                var driver = dynamicsContext.dfp_drivers.Where(x => x.dfp_driverid == driverId).FirstOrDefault();
                dynamicsContext.LoadProperty(driver, nameof(dfp_driver.dfp_PersonId));



                dynamicsContext.DeleteObject(driver);
                dynamicsContext.SaveChanges();
                if (driver.dfp_PersonId != null)
                {
                    dynamicsContext.Detach(driver.dfp_PersonId);
                }

                dynamicsContext.Detach(driver);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void DeleteCase(Guid id)
        {
            incident sparseCase = dynamicsContext.incidents.Where(x => x.incidentid == id).FirstOrDefault();
            if (sparseCase != null)
            {
                dynamicsContext.DeleteObject(sparseCase);
                dynamicsContext.SaveChanges();
            }

        }

    }

    

  

}
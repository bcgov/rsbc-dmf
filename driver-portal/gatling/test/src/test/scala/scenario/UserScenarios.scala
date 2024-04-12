package scenario

import requests.{AuthRequest, CaseRequest, DriverRequest, DocumentTypeRequest}
import io.gatling.core.Predef._
import config.Configuration
import io.gatling.http.Predef._

object UserScenarios {
          
    val getMostRecentCase = scenario("Get most recent case")
        .exec(CaseRequest.getMostRecentCase)

    val getDriverInfo = scenario("Get driver info")
        .exec(DriverRequest.getDriverInfo)

    val getClosedCases = scenario("Get closed cases")
        .exec(CaseRequest.getClosedCases)

    val getCaseDocuments = scenario("Get case documents")
        .exec(DriverRequest.getCaseDocuments)

    val getAllDocuments = scenario("Get all documents")
        .exec(DriverRequest.getCaseDocuments)

    val getDocumentTypes = scenario("Get document types")
        .exec(DocumentTypeRequest.getDocumentTypes)
}

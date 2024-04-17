package requests

import config.Configuration
import io.gatling.core.Predef._
import io.gatling.http.Predef._

object DriverRequest {
    
    val getDriverInfo = exec(http("Get driver info")
        .get("Driver/info")
        .header("Authorization", s"Bearer ${Configuration.bearerToken}")
        .check(status.is(200))
        .check(jsonPath("$.resultStatus").not("Fail"))
        //.check(responseTimeInMillis.lte(7000))
    )

    val getCaseDocuments = exec(http("Get case documents")
        .get("Driver/Documents")
        .header("Authorization", s"Bearer ${Configuration.bearerToken}")
        .check(status.is(200))
        .check(
            jmesPath("submissionRequirements").ofType[Seq[Any]],
            jmesPath("caseSubmissions").ofType[Seq[Any]],
            jmesPath("lettersToDriver").ofType[Seq[Any]]
        )
        //.check(responseTimeInMillis.lte(7000))
    )

    val getAllDocuments = exec(http("Get all documents")
        .get("Driver/AllDocuments")
        .header("Authorization", s"Bearer ${Configuration.bearerToken}")
        .check(status.is(200))
        .check(jsonPath("$").ofType[Seq[Any]])
        //.check(responseTimeInMillis.lte(7000))
    )
}
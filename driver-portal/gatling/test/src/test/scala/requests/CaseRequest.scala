package requests

import config.Configuration
import io.gatling.core.Predef._
import io.gatling.http.Predef._

object CaseRequest {
    
    val getMostRecentCase = exec(http("Get most recent case")
        .get("Cases/MostRecent")
        .header("Authorization", s"Bearer ${Configuration.bearerToken}")
        .check(status.is(200))
        .check(jsonPath("$.outstandingDocuments").is("0"))
        //.check(responseTimeInMillis.lte(10000))
    )

    val getClosedCases = exec(http("Get closed cases")
        .get("Cases/Closed")
        .header("Authorization", s"Bearer ${Configuration.bearerToken}")
        .check(status.is(200))
        .check(jsonPath("$").ofType[Seq[Any]])
        // TODO when not empty array, check the json properties like in all documents below
        //.check(jsonPath("$.?[?(@.id)]"))
        //.check(responseTimeInMillis.lte(10000))
    )
}
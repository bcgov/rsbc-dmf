package requests

import config.Configuration
import io.gatling.core.Predef._
import io.gatling.http.Predef._

object DocumentTypeRequest {
    
    val getDocumentTypes = exec(http("Get document types")
        .get("DocumentType/driver")
        .header("Authorization", s"Bearer ${Configuration.bearerToken}")
        .check(status.is(200))
        .check(
            jsonPath("$").ofType[Seq[Any]], //.count.is(9)
            jmesPath("[0].id").ofType[Int],
            jmesPath("[0].name").ofType[String],
        )
        //.check(responseTimeInMillis.lte(7000))
    )
}
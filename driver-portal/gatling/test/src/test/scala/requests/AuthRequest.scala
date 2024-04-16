import java.io.{BufferedWriter, FileWriter}

import config.Configuration
import io.gatling.core.Predef._
import io.gatling.http.Predef._

package requests {
    object AuthRequest {

        // refresh token
        val getRefreshToken = 
            addCookie(Cookie("mycookie", "<copy cookie here>")
                .withDomain("dev.roadsafetybc.gov.bc.ca"))
            http("Refresh token")
                .post(Configuration.tokenPath)
                .formParam("grant_type", "refresh_token")
                .formParam("client_id", Configuration.clientId)
                .formParam("refresh_token", "<copy refresh token here>")
                .formParam("scope", Configuration.scope)
                .formParam("acr_values", "idp:bcsc")
                .check(status.is(200))
                .check(jsonPath("$.access_token").saveAs("access_token"))
                .check(jsonPath("$.refresh_token").saveAs("refresh_token"))

    //     http("Authenticate in Keycloak")
    // .post("http://localhost:8080/token")
    // .asFormUrlEncoded()
    // .formParam("grant_type", "client_credentials")
    // .formParam("client_id", "my-client")
    // .formParam("client_secret", "secret")
    // .check(jmesPath("access_token").ofString()
    //     .exists().saveAs("access_token"));


        // .body(StringBody(
        // s"""{
        //     "client_id": "${Configuration.clientId}",
        //     "client_secret": "${Configuration.clientSecret}",
        //     "audience": "https://localhost",
        //     "grant_type": "client_credentials",
        //     "scope": "openid profile email offline_access driver-portal-api"
        //     }"""
        // ))
        //.check(status.is(302))

        // .asJson
        // .headers(Map("Content-Type" -> "application/json"))
        // .check(status.is(200))
        // .check(jsonPath("$.access_token").saveAs("access_token"))
        //)
        // .exec {
        // session =>
        //     val fw = new BufferedWriter(new FileWriter("access_token.txt", true))
        //     try {
        //     fw.write(session("access_token").as[String] + "\r\n")
        //     }
        //     finally fw.close()
        //     session
        // }
    }
}
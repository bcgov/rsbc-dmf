package config

object Configuration 
{
  val environment: String = System.getProperty("env", "dev")
  val clientId: String = "driver-portal-ui"
  val clientSecret: String = System.getProperty("secret")
  // NOTE needs to end with /
  val apiURL: String = s"https://${environment}.roadsafetybc.gov.bc.ca/driver-portal/api/"
  val tokenPath: String = s"https://${environment}.roadsafetybc.gov.bc.ca/rsbc-dfp-oidc/connect/token"
  val scope: String = "openid profile email offline_access driver-portal-api"
  // TODO get token
  // until then get the bearer token from the browser network tab e.g. MostRecent Authorization header
  val bearerToken: String = "FC809B3B2A4CA25C5DAEB229B5B960556ED72CBF777D54B822862FD69115E522"
}
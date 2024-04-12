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
  val bearerToken: String = "<copy bearer token here>"
}
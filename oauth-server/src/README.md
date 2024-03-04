## docker build

# oauth-server
REM change directory rsbc-dmf/oauth-server/src
REM add this to Startup.cs to replace the current UseCookiePolicy
app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

docker build . --tag oauth-server
docker run -p 5020:8080 --rm --name oauth-server \
  -e identityproviders:bcsc:ClientId=ca.bc.gov.pssg.dmfw.dev \
  -e identityproviders:bcsc:ClientSecret=qxmIIw6IEjFwD60ZlErK \
  -e identityproviders:bcsc:MetadataAddress=https://idtest.gov.bc.ca/login/.well-known/openid-configuration \
  -e https=null \
  -e ISSUER_URL=http://localhost:5020 \
  oauth-server

# EMBC ESS Landing page

The landing page is using openshift/caddy-template-service to serve the html located in /html folder.

## Build

1. Check in changes to the html in r1develop branch
2. in Pathfinder test project, trigger `caddy app` build, the deployment will happen automatically afterwards
3. to deploy production trigger `embcess-landing-page` build.

The html will be from the latest version in the `r1develop` branch.

# Openshift CLI

Use Template to perform the initial OpenShift setup and initial deployment. Subsequent builds and deployments triggered by GitHub Actions. Openshift token must be saved as a GitHub Action secret for this repo.

##
## 1. Build Image on GitHub & Push to OpenShift registry

### GitHub: Create Action Secrets
- `OPENSHIFT_REGISTRY_URL`      : image-registry.apps.silver.devops.gov.bc.ca
- `OPENSHIFT_TARGET_NAMESPACE`  : Namespace that contains the ImageStream (tools namespace)
- `OPENSHIFT_TOKEN`             : token for service account (see below)

### Run GitHub Action "build-pdf-service.yml"
Confirm Action runs without errors

##
## 2. Common Deployment Setup

### Setup ConfigMap (standalone, created once per environment)
```bash
oc apply -f configmap.yaml -n <runtime ns>
```

### Openshift: Ensure pdf-service image is in registry (ImageStreams)
Note: We created the the image and pushed it to the registry (tools namespace) with the build-pdf-service.yml 
```bash
oc project <image ns>
oc describe is/pdf-service
```
### Set policy to allow runtime namespace to pull from image namespace
```bash
oc policy add-role-to-user system:image-puller system:serviceaccount:<runtime ns>:default -n=<image ns>
```

### Create Deployment in the runtime namespace
```bash
oc process -p IMAGE_NS=<image-ns> -f deploy.yaml | oc apply -n <runtime-ns> -f -
```

### If you update the deployment yaml you can reapply it
```bash
oc apply -f deploy.yaml -n <runtime-ns>
```

### Check openshift to confirm service is deployed an running as expected

##
## 3. Usefull commands and information

## add redeploy trigger for when image changes
## Not needed with this template, already included
```bash
oc set triggers deploy/pdf-service --from-image=pdf-service:dev -c pdf-service
```

## Generate / fetch OpenShift token for GitHub actions.
This Allows GitHub Action to push the image to the OpenShift image registry

### Check if Service account exists
 ```
 oc get sa github-action-sa
 ```

### Create service account (if not already there)
```
oc create serviceaccount -n <tools-ns> github-action-sa
oc policy add-role-to-user edit -z github-action-sa -n <tools-ns>
```

### Create a long-lived token for this sa
```
oc apply -f sa_token.yaml
```

### Fetch the long-lived token for the sa
```
oc get secret github-action-sa-token -n <tools-ns> -o jsonpath='{.data.token}' | base64 -d
```

###  Test token in https://jwt.io to confirm no expiry

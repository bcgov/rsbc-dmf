# Formio PDF Render Service

A microservice that renders PDF versions of Form.io-based forms using dynamic templates and data. 
This service supports basic formio components and pre-fills form data, generating a PDF output.

## Features

- Dynamically loads and renders a Form.io template from a URL or cache.
- Pre-fills form data passed in the request.
- Basic components support via template manipulation.
- Generates PDFs from rendered forms.
- Configurable for local development and production environments (e.g., OpenShift).

## Prerequisites

Before you start, ensure you have the following installed:

- Node.js (v22+)
- NPM (10.9.2 or higher)

## Setup

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/formio-pdf-render-service.git
cd formio-pdf-render-service
```

### 2. Service folder structure
``` bash
app/
│
├── fallback/
│   └── defaultTemplate.json               # 🆘 Local fallback template
│
├── src/
│   ├── routes/
│   │   ├── renderRoute.ts                 # ↗️ /render endpoint, marry form template with data and output PDF
│   │   ├── reloadRoute.ts                 # ↗️ /reload-template endpoint, fetch remote tempalte and cache it
│   │   └── healthCheckRoute.ts            # ↗️ /healthcheck endpoint, openshift healtcheck
│   │
│   ├── services/
│   │   ├── initializeTemplate.ts          # 📡 Fetch remote template and cache
│   │   └── templateCache.ts               # 🔄 Manage template caching
│   │
│   ├── types/
│   │   └── formTemplate.ts                # 🖥️ Interfaces for formio template
│   │
│   ├── utils/
│   │   ├── extractSchema.ts               # 📋 Extracts formio template from downloaded template
│   │   ├── renderFormToPDF.ts             # 📑 Renders html form and produces PDF
│   │   ├── replaceCustomComponents.ts     # ♻️ Replaces formio custom components with basic ones
│   │   └── stripFormLogic.ts              # 🗑️ Strips form logic not required for PDF rendering
│   │    
│   ├── server.ts                          # 🚀 Main microservice entrypoint
│   └── config.ts                          # 🔧 Configuration object
│
├── static/
│   └── formio/
│       ├── createform.html                # 📡 Fetch remote template and cache
│       ├── formio.full.min.css            # 🎨 formio stylesheeet
│       └── formio.full.min.js             # 📜 formio script generates html from template and data
│
├── .env                                   # 🌱 Environment config (LOG_LEVEL, TEMPLATE_URL, etc.)
├── Dockerfile                             # 🐳 Markup to build docker image
├── openapi.yaml                           # 🔧 Openapi 3.x schema, endpoints definition (Swagger)
├── .gitignore
├── package.json
├── tsconfig.json                          # ⚙️ typescript settings
└── README.md                              # 📖 Documentation

```

### 3. Install dependencies
```bash
npm install
```

### 4. Environment Configuration
Create a .env file at the root of your project with the following keys:
```
PORT=3000
TEMPLATE_URL=https://submit.digital.gov.bc.ca/app/api/v1/forms/5383fc89-b219-49a2-924c-251cd1557eb8/version1
LOG_LEVEL=dev
```

### 5. Static Files
Ensure the static files (like form templates, CSS, and JS) are in place. These files are served from the /static route.

### 6. Start the Service
To run the service locally, use the following command:
```bash
npm run dev
```
By default, the service will be available at http://localhost:3000.

### 7. Access the Form
The form to be rendered to PDF is available at:
``` bash
http://localhost:3000/static/formio/createform.html

```

## API Endpoints
``` text
| Method | Endpoint             | Description                                                                                     |
|--------|----------------------|-------------------------------------------------------------------------------------------------|
| GET    | `/healthcheck`       | Checks if the service is running (returns `200 OK`).                                            |
| POST   | `/render`            | Submits JSON data to render a PDF using the cached form template. Requires a body like:         |
|        |                      | ```json<br>{ "data": { ... } }<br>```                                                           |
| POST   | `/reload-template`   | Triggers a manual reload of the template from the configured remote URL or fallback.            |
| GET    | `/api-docs`          | Swagger interactive UI API documentation. Swaffer/OpenAPI specification.                        |
```

### GET /healthcheck
This endpoint is used to check the health status of the service. It provides a simple way to verify that the service is running properly.

Response:

- 200 OK: The service is healthy and running.

Example Request
``` bash
curl http://localhost:3000/healthcheck
```
Example Response:
``` json
{
	"status": "OK",
	"message": "Formio service is healthy."
}
```

### POST /render
This endpoint accepts JSON data and renders the associated form as a PDF.

Request Body:
``` json
{
  "data": {
    "field1": "value1",
    "field2": "value2",
    ...
  }
}
```
**Response**:

- 200 OK: Returns the generated PDF as a Buffer in the response.

- 400 Bad Request: If there are validation errors or if the template is unavailable.

- 500 Internal Server Error: If there is a server-side issue.

- 503 "Service Unavailable: The server is not ready to handle your request at this moment. 

### Example Request
``` bash
curl -X POST http://localhost:3000/render \
  -H "Content-Type: application/json" \
  -d '{"data": {"corsConfiguredOrigin": "https://dev.roadsafetybc.gov.bc.ca", "initChefsForm": "false", "checkIsCommercialDMER": true}}' \
  --output form.pdf
```
This will save the generated PDF as form.pdf.


### `POST /reload-template`

This endpoint allows you to reload the form template from the configured remote `TEMPLATE_URL`. This is useful when you want to refresh the cached template, for example, after an update to the template without having to restart the service.

**Response**:
- `200 OK`: The template was successfully reloaded.
- `500 Internal Server Error`: If there was an error while fetching or caching the template.

### Example Request

```bash
curl -X POST http://localhost:3000/reload-template
```

### Example Response:
``` json
{
	"status": "OK",
	"message": "Template reloaded."
}
```

### Swagger web UI for API endpoint documentation
 Allows to read and explore available endpoints of the pdf-service with a browser.
 * Local:  `http://localhost:3000/api-docs/`
 * Remote: `http://<server address>/api-docs/`

## Deployment
The service is designed to run on various environments, including local and OpenShift. To deploy in OpenShift, follow the standard steps for deploying a Node.js application. The URLs for static assets and forms will be adjusted automatically based on the environment.

### Build & Run Container Image on localhost: Local

* build image: `podman build -t formio-pdf-service -f Dockerfile`
* run as container: `podman run --name formio-pdf --rm -d -p 3000:3000 formio-pdf-service`

#### Verify Running Container
* Local:  `http://localhost:3000/healthcheck`
* Remote: `http://<server address>/healthcheck`


### OpenShift
See README in `.openshift` dir


## Troubleshooting
Missing Bottom Part of PDF
If the PDF is rendering but the bottom part is missing, you can adjust the rendering logic by waiting for the form to load fully using waitForSelector or adding a slight delay with waitForTimeout().

"Validation failed" Errors
Ensure that the data passed in the request body matches the expected format of the form template. Double-check the data structure.



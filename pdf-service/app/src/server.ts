import dotenv from 'dotenv';
import express from 'express';
import morgan from 'morgan';
import path from 'path';
import swaggerUi from 'swagger-ui-express';
import YAML from 'yamljs';
import { config } from './config';
// Import routes
import renderRoute from './routes/renderRoute';
import reloadRoute from './routes/reloadRoute';
import healthCheckRoute from './routes/healthCheckRoute';
import { initializeTemplate } from './services/initializeTemplate';

dotenv.config();

const app = express();

// Load Swagger spec
const swaggerDocument = YAML.load(path.join(__dirname, '../openapi.yaml'));

// Morgan HTTP request logging
app.use(morgan(config.LOG_LEVEL));

// Accept JSON body
app.use(express.json());

// set the static assets 
app.use('/static', express.static(path.join(__dirname, '../static')));

// Swagger docs
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocument));

// Use routes
app.use(renderRoute);
app.use(reloadRoute);
app.use(healthCheckRoute);

// Start the server
const server = app.listen(config.PORT, async () => {
  console.log(`🚀 Formio render microservice running on port ${config.PORT}`);
  console.log(`📘 Swagger docs available at http://localhost:${config.PORT}/api-docs`);
  try {
    // Load and cache the form template at startup
    await initializeTemplate(config.TEMPLATE_URL);
    console.log('✅ Template initialized successfully');
  } catch (err) {
    console.error('⚠️ Failed to load template at startup:', err);
  }
});

server.on('error', (err: any) => {
  if (err.code === 'EADDRINUSE') {
    console.error(`❌ Port ${config.PORT} is already in use. Choose a different port.`);
    process.exit(1);
  } else {
    console.error('❌ Server error:', err);
    process.exit(1);
  }
});

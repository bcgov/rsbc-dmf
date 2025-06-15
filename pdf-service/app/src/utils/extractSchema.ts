import { IFormioTemplate } from '../types/serviceInterfaces';

// Helper function to extract the schema from the raw template
export function extractSchema(template: any): IFormioTemplate {
  // Assuming the template has a 'versions' array and we want the schema from the first version
  if (template && template.versions && Array.isArray(template.versions)) {
    // Taking the first version, we can add additional logic to handle different versions
    const version = template.versions[0]; 
    if (version && version.schema) {
      return version.schema;
    }
  }

  console.error('❌ No schema found in template!');
  throw new Error('Invalid template format');
}

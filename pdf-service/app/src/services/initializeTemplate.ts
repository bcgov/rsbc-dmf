import axios from 'axios';
import path from 'path';
import fs from 'fs';
import { FormioTemplate } from '../types/formioTemplate'; // Make sure to import your FormioTemplate type
import { replaceCustomComponents } from '../utils/replaceCustomComponents';
import { stripFormLogic } from '../utils/stripFormLogic';
import { setCachedTemplate } from './templateCache';
import { extractSchema } from '../utils/extractSchema';

export async function initializeTemplate(templateUrl: string): Promise<FormioTemplate | null> {
  try {
    console.log('📡 Fetching form template from:', templateUrl);

    // Fetch the remote template via axios
    const response = await axios.get<FormioTemplate>(templateUrl);

    // Extract the form schema from the downloaded template
    const schema = extractSchema(response.data);

    // Clean the template (replace components and strip logic)
    const cleaned = stripFormLogic(replaceCustomComponents(schema));

    // Cache the cleaned template
    setCachedTemplate(cleaned);

    console.log('✅ Template loaded and cached from remote');

    // Return the cleaned template
    return cleaned;
  } catch (err) {
    console.error('⚠️ Failed to fetch remote template, using local fallback', (err as Error).message);

    // Fallback to local template if remote fetch fails
    const fallbackPath = path.join(__dirname, '../../fallback/defaultTemplate.json');

    if (fs.existsSync(fallbackPath)) {
      try {
        const fallbackTemplate = JSON.parse(fs.readFileSync(fallbackPath, 'utf-8')) as FormioTemplate;

        // Extract the form schema from fallback template
        const fallbackSchema = extractSchema(fallbackTemplate);

        // Clean the fallback template (replace components and strip logic)
        const cleaned = stripFormLogic(replaceCustomComponents(fallbackSchema));

        // Cache the fallback template
        setCachedTemplate(cleaned);

        console.log('✅ Local fallback template loaded and cached');

        // Return the cleaned fallback template
        return cleaned;
      } catch (readErr) {
        console.error('❌ Failed to read or process fallback template', readErr);
        throw new Error('Template initialization failed');
      }
    } else {
      console.error('❌ No fallback template available at', fallbackPath);
      throw new Error('Template initialization failed');
    }
  }
}

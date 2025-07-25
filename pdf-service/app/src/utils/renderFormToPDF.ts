import { chromium } from 'playwright';
import { getCachedTemplate } from '../services/templateCache';
import { processTemplate } from '../services/processTemplate';
import { extarctFirstQuestionLookup } from './extractFirstQuestionLookup';
import { config } from '../config';

export async function renderFormToPdf(data: any): Promise<Buffer> {
  console.time('🕒 Total PDF render time');

  const cachedTemplate = getCachedTemplate();
  if (!cachedTemplate) {
    throw new Error('Form template not available in cache.');
  }

  const processedTemplate = processTemplate(cachedTemplate, extarctFirstQuestionLookup(data));

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage();

  // Block unnecessary resources
  await page.route('**/*', (route) => {
    const blocked = ['image', 'font'];
    if (blocked.includes(route.request().resourceType())) {
      return route.abort();
    }
    route.continue();
  });

  // Preload form template and data via localStorage
  await page.addInitScript(([formTemplate, submissionData]) => {
    window.localStorage.setItem('formTemplate', JSON.stringify(formTemplate));
    window.localStorage.setItem('submissionData', JSON.stringify(submissionData));
  }, [processedTemplate, data]);

  const formUrl = `http://localhost:${config.PORT}/static/formio/createform.html`;
  await page.goto(formUrl, { waitUntil: 'networkidle' });

  // Wait for the form to render by checking the form's container element
  await page.waitForSelector('#form-render-complete', { timeout: 10000, state: 'attached' });

  const pdfBuffer = await page.pdf({
    format: 'A4',
    printBackground: true,
    margin: {
      top: '0.5cm',
      bottom: '0.5cm',
      left: '0.1cm',
      right: '0.1cm'
    }
  });

  await browser.close();
  console.timeEnd('🕒 Total PDF render time');
  return pdfBuffer;
}

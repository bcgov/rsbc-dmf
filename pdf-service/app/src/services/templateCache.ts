import { FormioTemplate } from '../types/formioTemplate';

let cachedTemplate: FormioTemplate | null = null;

export function setCachedTemplate(template: FormioTemplate): void {
  cachedTemplate = template;
}

export function getCachedTemplate(): FormioTemplate | null {
  return cachedTemplate;
}

export function isTemplateReady(): boolean {
  return cachedTemplate !== null;
}

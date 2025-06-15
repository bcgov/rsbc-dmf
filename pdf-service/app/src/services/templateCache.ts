import { IFormioTemplate } from '../types/serviceInterfaces';

let cachedTemplate: IFormioTemplate | null = null;

export function setCachedTemplate(template: IFormioTemplate): void {
  cachedTemplate = template;
}

export function getCachedTemplate(): IFormioTemplate | null {
  return cachedTemplate;
}

export function isTemplateReady(): boolean {
  return cachedTemplate !== null;
}

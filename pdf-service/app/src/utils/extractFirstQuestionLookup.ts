import { IJsonData } from '../types/serviceInterfaces';

/**
 * Extracts all key-value pairs from jsonData where the key matches
 * eDMER<number>FirstQuestionAnswered, and the value is a boolean true.
 *
 * @export
 * @param {IJsonData} jsonData 
 * @returns {Set<string>} 
 */
export function extarctFirstQuestionLookup(jsonData: IJsonData): Set<string> {
  const keys = new Set<string>();
  const prefix = "eDMER";
  const suffix = "FirstQuestionAnswered";
  for (const key in jsonData.data) {
    if (!key.startsWith(prefix) || !key.endsWith(suffix)) continue;

    const middlePart = key.slice(prefix.length, key.length - suffix.length);
    if (middlePart === "" || isNaN(Number(middlePart))) continue;

    const value = jsonData.data[key];
    if (value === true) {
      const baseKey = `${prefix}${middlePart}`;
      keys.add(baseKey);
    }
  }
  return keys;
}

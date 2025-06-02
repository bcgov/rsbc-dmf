/**
 * Recursively strips logic-related properties from Form.io components
 * to simplify a form definition for static rendering.
 */
// import fs from 'fs';
// import path from 'path';

export function stripFormLogic(template: any): any {
  let removedCount = 0;
  const removedDetails: Record<string, string[]> = {};

  const removeKeys = [
    'action', 'custom', 'customDefaultValue', 'refreshOn', 'redrawOn',
    'calculateServer', 'allowCalculateOverride', 'disableOnInvalid',
    'autofocus', 'encrypted', 'protected', 'modalEdit', 'dataGridLabel',
    'persistent', 'tabindex', 'tooltip', 'customClass', 'hideLabel',
    'leftIcon', 'rightIcon', 'errorLabel', 'properties', 'clearOnHide',
    'validateOn', 'calculateValue', 'debugPanel',
    // UI & layout specific extras
    'addons', 'prefix', 'suffix', 'widget', 'dbIndex', 'overlay',
    'disabled', 'multiple', 'tableView', 'description', 'placeholder',
    'defaultValue', 'labelPosition', 'showCharCount', 'showWordCount',
    'showValidations', 'allowMultipleMasks', 'spellcheck', 'case',
    'autocomplete', 'truncateMultipleSpaces', 'errors', 'attributes',
    'inputFormat', 'inputMask', 'mask', 'unique', 'hidden', 'breadcrumb'
  ];

  const buttonTypes = ['button', 'submit', 'reset'];

  function cleanComponent(component: any): any {
    // Skip button-like components entirely
    if (buttonTypes.includes(component.type)) {
      removedCount++;
      removedDetails[component.key || component.id || 'unknown'] = ['entire button component'];
      return null;
    }

    // Remove components with key === "debugPanel"
    if (component.key === 'debugPanel') {
        removedCount++;
        removedDetails[component.key || component.id || 'unknown'] = ['removed debugPanel component'];
        return null;
    }

    // Skip hidden fields
    if (component.hidden === true) {
        removedCount++;
        removedDetails[component.key || component.id || 'unknown'] = ['hidden field removed'];
        return null;
    }

    // Skip warning-themed panels
    if (component.type === 'panel' && component.theme === 'warning') {
        removedCount++;
        removedDetails[component.key || component.id || 'unknown'] = ['removed panel with theme "warning"'];
        return null;
    }

    // Skip keys containing "4000"
    if (typeof component.key === 'string' && component.key.includes('4000')) {
        removedCount++;
        removedDetails[component.key] = ['key includes "4000"'];
        return null;
    }

    // Skip simpletextfield components
    if (component.type === 'simpletextfield') {
        removedCount++;
        removedDetails[component.key || component.id || 'unknown'] = ['removed type "simpletextfield"'];
        return null;
    }

    const cleaned: any = {};
    const removedKeys: string[] = [];

    for (const [key, value] of Object.entries(component)) {
        if (removeKeys.includes(key)) {
          removedKeys.push(key);
          removedCount++;
        } else if (key === 'validate' && typeof value === 'object' && value !== null) {
          const { custom, ...restValidate } = value as Record<string, any>;
          const isEffectivelyEmpty = Object.values(restValidate).every(
            v => v === false || v === '' || v === null || v === undefined
          );
      
          if (!custom && !isEffectivelyEmpty) {
            cleaned.validate = restValidate;
          } else {
            removedKeys.push('validate');
            removedCount++;
          }
        } else {
          cleaned[key] = value;
        }
    }

    // Special case: Remove generic panel label
    if ((component.type === 'panel' || component.type === 'simplepanel') && component.label === 'Panel') {
        delete cleaned.label;
        removedDetails[component.key || component.id || 'unknown'] = [
        ...(removedDetails[component.key || component.id || 'unknown'] || []),
        'label (generic panel)'
        ];
        removedCount++;
    }

    // Track what we removed
    const identifier = component.key || component.id || 'unknown';
    if (removedKeys.length > 0) {
      removedDetails[identifier] = removedKeys;
    }

    // Recursively clean nested structures
    if (component.components && Array.isArray(component.components)) {
      cleaned.components = component.components.map(cleanComponent).filter(Boolean);
    }

    if (component.columns && Array.isArray(component.columns)) {
      cleaned.columns = component.columns.map((column: any) => ({
        ...column,
        components: column.components?.map(cleanComponent).filter(Boolean) || [],
      }));
    }

    if (component.rows && Array.isArray(component.rows)) {
      cleaned.rows = component.rows.map((row: any[]) =>
        row
          .map((col: any) => ({
            ...col,
            components: col.components?.map(cleanComponent).filter(Boolean) || [],
          }))
          .filter(Boolean)
      );
    }

    if (component.fieldSet && Array.isArray(component.fieldSet)) {
      cleaned.fieldSet = component.fieldSet.map(cleanComponent).filter(Boolean);
    }

    return cleaned;
  }

  const strippedTemplate = {
    ...template,
    components: template.components?.map(cleanComponent).filter(Boolean) || [],
  };

//   const debugPath = path.resolve(process.cwd(), 'stripped-form.debugZ4.json');
//   fs.writeFileSync(debugPath, JSON.stringify(strippedTemplate, null, 2));

//   fs.writeFileSync(path.resolve(process.cwd(), 'stripped-form.debugZ1.min.json'), JSON.stringify(strippedTemplate));

//  console.log(`🧹 Removed ${removedCount} dynamic/UI properties or components.`);
//   for (const [id, keys] of Object.entries(removedDetails)) {
//     console.log(`  🔧 Component "${id}": stripped [${keys.join(', ')}]`);
//   }

  return strippedTemplate;
}


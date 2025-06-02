// import fs from 'fs';
// import path from 'path';

const typeMap: Record<string, string> = {
    simplecheckboxadvanced: 'checkbox',
    simpleradioadvanced: 'radio',
    simpletextareaadvanced: 'textarea',
    simpletextfieldadvanced: 'textfield',
    simpleselectboxesadvanced: 'selectboxes',
    simplebuttonadvanced: 'button',
    simplenumberadvanced: 'number',
    simpledayadvanced: 'day',
  };
  
  export function replaceCustomComponents(template: object): object {
    let replacementCount = 0;
  
    function traverse(node: any): any {
      if (Array.isArray(node)) {
        return node.map(traverse);
      }
  
      if (node && typeof node === 'object') {
        const newNode: any = {};
        for (const [key, value] of Object.entries(node)) {
          if (key === 'type' && typeof value === 'string' && typeMap[value]) {
            newNode[key] = typeMap[value];
            replacementCount++;
          } else {
            newNode[key] = traverse(value);
          }
        }
        return newNode;
      }
  
      return node;
    }
  
    const replacedTemplate = traverse(template);

    
//   // Output debug file
//   const debugPath = path.resolve(process.cwd(), 'replaced-components.debug.json');
//   fs.writeFileSync(debugPath, JSON.stringify(replacedTemplate, null, 2));
  
    // console.log(`🔁 Replaced ${replacementCount} custom component type(s).`);
  
    return replacedTemplate;
  }

/*
The following variables are available in all scripts:
token	      - The decoded JWT token for the authenticated user.
user	      - The currently logged in user
form	      - The complete form JSON object
submission	- The complete submission object.
data	      - The complete submission data object.
row	        - Contextual "row" data, used within DataGrid, EditGrid, and Container components
component	  - The current component JSON
instance	  - The current component instance.
value	      - The current value of the component.
moment	    - The moment.js library for date manipulation.
_	          - An instance of Lodash.
utils	      - An instance of the FormioUtils object.
util	      - An alias for "utils".
*/

var GET_CHEFS_SUBMISSION = "GET_CHEFS_SUBMISSION";
var GET_CHEFS_BUNDLE = "GET_CHEFS_BUNDLE";
var PUT_CHEFS_SUBMISSION = "PUT_CHEFS_SUBMISSION";
// value comes form "CORS Configured Origin" text field on chefs form
var cors_origin = data.corsConfiguredOrigin || "https://dev.roadsafetybc.gov.bc.ca";

window.isSubmitted = window.isSubmitted || false;

// Early return if form is already submitted
if (window.isSubmitted) {
  console.warn("window has been already submitted.");
  return;
}

if (!window || !window.parent || !window.parent.postMessage || !data) return;

// Define a flag to track the event listener on the window object
window.listenerAdded = window.listenerAdded || {};

// return instanceId if already saved, otherwise get the instanceId from querystring and save it to this instance (window object)
function getInstanceId() {
  if (window.instanceId) return window.instanceId;
  const currentUrl = window.location.href;
  const url = new URL(currentUrl);
  const params = new URLSearchParams(url.search);
  const instanceId = params.get("instanceId");
  if (instanceId) window.instanceId = instanceId;
  return instanceId;
}

// Function to add a unique event listener to the window
function addUniqueWindowEventListener(event, listener) {
  // Check if the listener for the specific event has already been added
  if (!window.listenerAdded[event]) {
    console.info(`[IFRAME] addUniqueWindowEventListener: listener for event: ${event} has not yet been added, adding...`);
    window.addEventListener(event, listener);
    window.listenerAdded[event] = true; // Update the flag for this event
  } else {
    console.warn(`[IFRAME] addUniqueWindowEventListener: listener for event: ${event} already added, do nothing.`);
  }
}

// Add message event listener to the window
addUniqueWindowEventListener("message", (event) => {
  if (!event || !event.data) {
    console.error("[IFRAME] Failed to parse event data payload");
    return;
  }

  console.info("[IFRAME] RX (from host): Iframe received message event:", event);

  // NOTE comment this out for LOCAL chefs form which is only used for localhost testing
  if (event.origin !== cors_origin)
  {
    console.warn(`[IFRAME] Ignore event from unrecognized origin: ${event.origin}`);
    return;
  }

  try {
    const payload = JSON.parse(event.data);

    const { type } = payload;

    console.info(`[IFRAME] RX (from host): Received response for type: ${type} the data:`, payload);

    if (type === GET_CHEFS_SUBMISSION) {
      window.chefsSubmission = payload;
      // load current cached version into memory for comparison later
      cacheCurrentSubmissionData(payload.submission);

      // load data into current form
      loadPreviousChefsSubmissionData(payload.submission || {});
    } else if (type === GET_CHEFS_BUNDLE) {
      window.chefsBundle = payload;

      // load fetched bundle data w/driver information
      loadChefsBundleData(payload || {});
    }
  } catch (e) {
    console.error("[IFRAME] Problem parsing event data JSON payload", e);
    return;
  }
});

function checkIfInitIsComplete() {
  if (window.loadedChefsSubmission && window.loadedChefsBundle) {
    console.info("[IFRAME] CHEFS FORM INIT COMPLETE!");
    window.initComplete = true;

    // call putChefsSubmission in case cached data differs vs. bundle + cached data
    putChefsSubmission();
  } else {
    console.warn(`[IFRAME] checkIfInitIsComplete: not complete yet, window.loadedChefsSubmission: ${window.loadedChefsSubmission}, window.loadedChefsBundle: ${window.loadedChefsBundle}!`);
  }
}

function loadChefsBundleData(fetchedBundleData) {
  console.info("[IFRAME] START LOADING FETCHED BUNDLE DATA:", fetchedBundleData);
  if (!fetchedBundleData?.patientCase || !fetchedBundleData?.driverInfo) {
    console.warn(`[IFRAME] Missing chefs bundle data, either missing patientCase or driverInfo`);
  }

  const {
    patientCase: { driverLicenseNumber },
    driverInfo: { surname, birthDate, licenceClass },
    medicalConditions,
    dmerType
  } = fetchedBundleData;

  const values = {
    driverLicenseNumber,
    surname,
    birthDate,
    medicalConditions,
    dmerType,
    licenceClass
  };

  Object.keys(values).forEach((key) => {
    if (!values[key]) {
      console.error(`[IFRAME] Attempt to set chefsKey: ${key} FAILED, undefined value`);
      return;
    }

    console.info(`[IFRAME] SETTING... chefsKey: ${key}, value: ${values[key]}`);
    console.info(`RAWDATA:`, values[key]);

    // for the special case of medicalConditions bundle data, loop through each condition
    // and set the matching CHEFS component checkbox to true if it's present
    if (key === "medicalConditions" && values[key].length > 0) {
      const flattenedComponents = getFlattenedComponents();
      values[key].forEach((medicalCondition) => {
        const { formId } = medicalCondition;
        const matchingMedicalConditionComponent = Object
          .values(flattenedComponents)
          .find((comp) => comp?.originalComponent?.properties?.kmcformid === formId);
        if (!matchingMedicalConditionComponent) {
          console.warn(`[IFRAME] loadChefsBundleData: could not find matching component for formId: ${formId}`);
          return;
        }

        console.info(`[IFRAME]: loadChefsBundleData: attempting to set matchingMedicalConditionComponent for formId: ${formId}...`);
        utils
          .getComponent(
            Formio.forms[window.currentFormId].components,
            matchingMedicalConditionComponent.key,
          )
          .setValue(true);
        console.info(`[IFRAME]: loadChefsBundleData: SUCCESSFULLY set matchingMedicalConditionComponent for formId: ${formId}...`);
      });
    } else {
      const component = utils.getComponent(
        Formio.forms[window.currentFormId].components,
        key,
      );

      if (!component) {
        console.warn(`[IFRAME] No component found with key: ${key}`);
        return;
      }

      utils
        .getComponent(Formio.forms[window.currentFormId].components, key)
        .setValue(values[key]);
    }
  });

  window.loadedChefsBundle = true;

  console.info("[IFRAME] FINISHED LOADING FETCHED BUNDLE DATA!");

  checkIfInitIsComplete();
}

function loadPreviousChefsSubmissionData(fetchedSubmissionData) {
  console.info("[IFRAME] START LOADING PREVIOUSLY CACHED SUBMISSION DATA:");
  if (
    !fetchedSubmissionData ||
    Object.keys(fetchedSubmissionData).length <= 0
  ) {
    console.warn("[IFRAME] No previous chefs submission data to load");
    window.loadedChefsSubmission = true;
    return;
  }

  Object.keys(fetchedSubmissionData).forEach((key) => {
    console.info(`[IFRAME] SETTING... key: ${key}, value: ${fetchedSubmissionData[key]}`);

    const component = utils.getComponent(
      Formio.forms[window.currentFormId].components,
      key,
    );

    if (!component) {
      console.warn(`[IFRAME] No component found with key: ${key}`);
      return;
    }

    utils
      .getComponent(Formio.forms[window.currentFormId].components, key)
      .setValue(fetchedSubmissionData[key]);
  });

  window.loadedChefsSubmission = true;

  console.info("[IFRAME] FINISH LOADING PREVIOUSLY CACHED SUBMISSION DATA!");

  checkIfInitIsComplete();
}

function cacheCurrentSubmissionData(submissionData) {
  const message = {
    instanceId: getInstanceId(),
    type: PUT_CHEFS_SUBMISSION,
    status: "Draft",
    submission: submissionData,
  };

  const messageObj = JSON.parse(JSON.stringify(message));

  window.cachedMessage = JSON.stringify(messageObj);
}

function getChefsSubmission() {
  const message = {
    instanceId: getInstanceId(),
    type: GET_CHEFS_SUBMISSION,
  };

  const messageObj = JSON.parse(JSON.stringify(message));

  window.parent.postMessage(messageObj, cors_origin); // Use a specific origin instead of '*' for better security
}

function getChefsBundle() {
  const message = {
    instanceId: getInstanceId(),
    type: GET_CHEFS_BUNDLE,
  };

  const messageObj = JSON.parse(JSON.stringify(message));

  window.parent.postMessage(messageObj, cors_origin); // Use a specific origin instead of '*' for better security
}

function getFlattenedComponents() {
  return utils.flattenComponents(Formio.forms[window.currentFormId].components);
}

function getCaseFlags() {
  const flattenedComponents = getFlattenedComponents();

  // get all components that contain a property called 'flagformid' which is only used by case flags
  const chefsFlagComponents = Object.values(flattenedComponents).filter(
    (comp) => comp?.originalComponent?.properties?.flagformid,
  );

  const flags = {};

  // add the flag 'flagformid' and the true/false value of the flag to the flags obj
  // this flags object is then processed in the backend and mapped to flags present in dynamics
  chefsFlagComponents.forEach((comp) => {
    const key = comp.originalComponent?.key;
    const flagformid = comp.originalComponent?.properties?.flagformid;

    console.info(`[IFRAME] getCaseFlags: attempting to add chefs flagformid: ${flagformid}...`);

    const value = utils
      .getComponent(Formio.forms[window.currentFormId].components, key)
      .getValue();

    if (flagformid) {
      flags[flagformid] = value;
      console.info(`[IFRAME] getCaseFlags: successfully added chefs flagformid: ${flagformid}, value: ${value}`);
    }
  });

  return flags;
}

function getPriority() {
  if (window.isSubmitted) {
    console.warn("window has been already submitted.");
    return;
  }

  const flattenedComponents = getFlattenedComponents();

  // get all components that contain a property called 'flagformid' which is only used by case flags
  const priorityComponents = Object.values(flattenedComponents).filter(
    (comp) => comp?.originalComponent?.properties?.priority,
  );

  const selectedPrioritiesComponents = [];

  // add the flag 'flagformid' and the true/false value of the flag to the flags obj
  // this flags object is then processed in the backend and mapped to flags present in dynamics
  priorityComponents.forEach((comp) => {
    const key = comp.originalComponent?.key;
    const priority = comp.originalComponent?.properties?.priority;

    const value = utils
      .getComponent(Formio.forms[window.currentFormId].components, key)
      .getValue();

    console.info(`[IFRAME] getPriority: attempting to add chefs priority: ${priority} with value: ${value}...`);

    if (value) {
      selectedPrioritiesComponents.push(comp);
      console.info(`[IFRAME] getPriority: successfully added chefs priority: ${priority}, value: ${value}`);
    }
  });

  if (
    selectedPrioritiesComponents.length === 0 ||
    selectedPrioritiesComponents.length > 1
  ) {
    console.error("[IFRAME] getPriority: Found zero or more than one component with priority attribute, failed to determine correct value", priorityComponents);
    console.error("[IFRAME] selectedPrioritiesComponents", selectedPrioritiesComponents);
    return "";
  }

  const priorityComponent = selectedPrioritiesComponents[0];

  const priority = priorityComponent.originalComponent?.properties?.priority;

  if (!priority) {
    console.error(`[IFRAME] getPriority: Failed to find priority attribute on component`, priorityComponents);
    return "";
  }

  console.info(`[IFRAME] getPriority: Successfully found priority with value: ${priority}!`);

  return priority;
}

function getAssign() {
  const flattenedComponents = getFlattenedComponents();

  // get all components that contain a property called 'flagformid' which is only used by case flags
  const assignComponents = Object.values(flattenedComponents).filter(
    (comp) => comp?.originalComponent?.properties?.assign,
  );

  const selectedAssignComponents = [];

  // add the flag 'flagformid' and the true/false value of the flag to the flags obj
  // this flags object is then processed in the backend and mapped to flags present in dynamics
  assignComponents.forEach((comp) => {
    const key = comp.originalComponent?.key;
    const assign = comp.originalComponent?.properties?.assign;

    const value = utils
      .getComponent(Formio.forms[window.currentFormId].components, key)
      .getValue();

    console.info(`[IFRAME] getAssign: attempting to add chefs assign: ${assign} with value: ${value}...`);

    if (value) {
      selectedAssignComponents.push(comp);
      console.info(`[IFRAME] getAssign: successfully added chefs assign: ${assign}, value: ${value}`);
    }
  });

  if (
    selectedAssignComponents.length === 0 ||
    selectedAssignComponents.length > 1
  ) {
    console.error(
      "[IFRAME] getAssign: Found zero or more than one component with assign attribute, failed to determine correct value",
    );
    console.error("selectedAssignComponents", selectedAssignComponents);
    console.error("assignComponents", assignComponents);
    return "";
  }

  const assignComponent = selectedAssignComponents[0];

  const assign = assignComponent.originalComponent?.properties?.assign;

  if (!assign) {
    console.error(`[IFRAME] getAssign: Failed to find assign attribute on component`, assignComponents);
    return "";
  }

  console.info(`[IFRAME] getAssign: Successfully found assign with value: ${assign}!`);

  return assign;
}

function putChefsSubmission(status = "Draft") {
  const flags = getCaseFlags();
  const priority = status === "Final" ? getPriority() : "";
  const assign = status === "Final" ? getAssign() : "";

  const message = {
    instanceId: getInstanceId(),
    type: PUT_CHEFS_SUBMISSION,
    status,
    submission: data,
    flags,
    priority,
    assign,
  };

  const messageObj = JSON.parse(JSON.stringify(message));

  if (window.cachedMessage === JSON.stringify(messageObj)) {
    console.info("[IFRAME] No change in form data, skip posting message to parent...");
    return;
  }

  // Send the message to the parent window
  window.parent.postMessage(messageObj, cors_origin); // Use a specific origin instead of '*' for better security
  window.cachedMessage = JSON.stringify(messageObj);
}

window.putChefsSubmission = putChefsSubmission;

// On initial load, fetch any bundle or existing draft submission to load in data
if (!window.hasLoaded) {
  console.info(`[IFRAME] Found currentFormId: ${Object.keys(Formio.forms)[0]}`);
  window.currentFormId = Object.keys(Formio.forms)[0];

  if (!window.currentFormId || window.currentFormId == 0) {
    console.info("[IFRAME] No form id found yet... waiting...");
    console.info(Formio.forms);
    return;
  }
  getChefsSubmission();
  getChefsBundle();
  window.hasLoaded = true;
}

if (window.initComplete) {
  // Call the function when you need to send the message and only when data is present
  putChefsSubmission();
}

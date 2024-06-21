/*
The following variables are available in all scripts.
token	The decoded JWT token for the authenticated user.
user	The currently logged in user
form	The complete form JSON object
submission	The complete submission object.
data	The complete submission data object.
row	Contextual "row" data, used within DataGrid, EditGrid, and Container components
component	The current component JSON
instance	The current component instance.
value	The current value of the component.
moment	The moment.js library for date manipulation.
_	An instance of Lodash.
utils	An instance of the FormioUtils object.
util	An alias for "utils".
*/
var GET_CHEFS_SUBMISSION = "GET_CHEFS_SUBMISSION";
var GET_CHEFS_BUNDLE = "GET_CHEFS_BUNDLE";
var PUT_CHEFS_SUBMISSION = "PUT_CHEFS_SUBMISSION";

window.isSubmitted = window.isSubmitted || false;

// Early return if form is already submitted
if (window.isSubmitted) return;

if (!window || !window.parent || !window.parent.postMessage || !data) return;

// Define a flag to track the event listener on the window object
window.listenerAdded = window.listenerAdded || {};

function getInstanceId() {
  if (window.instanceId) return window.instanceId;
  const currentUrl = window.location.href;
  const url = new URL(currentUrl);
  const params = new URLSearchParams(url.search);
  const instanceId = params.get("instanceId");
  window.instanceId = instanceId;
  return instanceId;
}

// Function to add a unique event listener to the window
function addUniqueWindowEventListener(event, listener) {
  // Check if the listener for the specific event has already been added
  if (!window.listenerAdded[event]) {
    console.log(
      `[IFRAME] addUniqueWindowEventListener: listener for event: ${event} has not yet been added, adding...`,
    );
    window.addEventListener(event, listener);
    window.listenerAdded[event] = true; // Update the flag for this event
  } else {
    console.log(
      `[IFRAME] addUniqueWindowEventListener: listener for event: ${event} already added, do nothing.`,
    );
  }
}

// Add the resize event listener to the window
addUniqueWindowEventListener("message", (event) => {
  console.log("[IFRAME] RX (from host): Iframe received message event:");
  console.log(event);

  if (event.origin !== "https://localhost:4200") {
    console.warn(
      `[IFRAME] Ignore event from unrecognized origin: ${event.origin}`,
    );
    return;
  }

  if (!event || !event.data) {
    console.error("[IFRAME] Failed to parse event data payload");
    return;
  }

  try {
    const payload = JSON.parse(event.data);

    const { type } = payload;

    console.log(
      `[IFRAME] RX (from host): Received response for type: ${type} the data:`,
    );
    console.log(payload);

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
    console.error("[IFRAME] Problem parsing event data JSON payload");
    console.error(e);
    return;
  }
});

function checkIfInitIsComplete() {
  if (window.loadedChefsSubmission && window.loadedChefsBundle) {
    console.log("[IFRAME] CHEFS FORM INIT COMPLETE!");
    window.initComplete = true;

    // call putChefsSubmission in case cached data differs vs. bundle + cached data
    putChefsSubmission();
  }
}

function loadChefsBundleData(fetchedBundleData) {
  console.log("[IFRAME] START LOADING FETCHED BUNDLE DATA:");
  console.log(fetchedBundleData);
  const {
    patientCase: { driverLicenseNumber },
    driverInfoReply: { surname, birthDate },
  } = fetchedBundleData;

  const values = {
    driverLicenseNumber,
    surname,
    birthDate,
  };

  const mapToApiKey = {
    textTargetDriverLicense: "driverLicenseNumber",
    textTargetDriverName: "surname",
    tDateTargetDriverBirthdate: "birthDate",
  };

  Object.keys(mapToApiKey).forEach((chefsKey) => {
    console.log(
      `[IFRAME] SETTING... chefsKey: ${chefsKey}, value: ${values[mapToApiKey[chefsKey]]}`,
    );

    const component = utils.getComponent(
      Formio.forms[window.currentFormId].components,
      chefsKey,
    );

    if (!component) {
      console.warn(`[IFRAME] No component found with key: ${chefsKey}`);
      return;
    }

    utils
      .getComponent(Formio.forms[window.currentFormId].components, chefsKey)
      .setValue(values[mapToApiKey[chefsKey]]);
  });

  window.loadedChefsBundle = true;

  console.log("[IFRAME] FINISHED LOADING FETCHED BUNDLE DATA!");

  checkIfInitIsComplete();
}

function loadPreviousChefsSubmissionData(fetchedSubmissionData) {
  console.log("[IFRAME] START LOADING PREVIOUSLY CACHED SUBMISSION DATA:");
  if (
    !fetchedSubmissionData ||
    Object.keys(fetchedSubmissionData).length <= 0
  ) {
    console.warn("[IFRAME] No previous chefs submission data to load");
    return;
  }

  Object.keys(fetchedSubmissionData).forEach((key) => {
    console.log(
      `[IFRAME] SETTING... key: ${key}, value: ${fetchedSubmissionData[key]}`,
    );

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

  console.log("[IFRAME] FINISH LOADING PREVIOUSLY CACHED SUBMISSION DATA!");

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

  window.parent.postMessage(messageObj, "https://localhost:4200/"); // Use a specific origin instead of '*' for better security
}

function getChefsBundle() {
  const message = {
    instanceId: getInstanceId(),
    type: GET_CHEFS_BUNDLE,
  };

  const messageObj = JSON.parse(JSON.stringify(message));

  window.parent.postMessage(messageObj, "https://localhost:4200/"); // Use a specific origin instead of '*' for better security
}

function putChefsSubmission(status = "Draft") {
  const message = {
    instanceId: getInstanceId(),
    type: PUT_CHEFS_SUBMISSION,
    status,
    submission: data,
  };

  const messageObj = JSON.parse(JSON.stringify(message));

  if (window.cachedMessage === JSON.stringify(messageObj)) {
    console.log(
      "[IFRAME] No change in form data, skip posting message to parent...",
    );
    return;
  }

  // Send the message to the parent window
  window.parent.postMessage(messageObj, "https://localhost:4200/"); // Use a specific origin instead of '*' for better security
  window.cachedMessage = JSON.stringify(messageObj);
}

window.putChefsSubmission = putChefsSubmission;

// On initial load, fetch any bundle or existing draft submission to load in data
if (!window.hasLoaded) {
  console.log(`[IFRAME] Found currentFormId: ${Object.keys(Formio.forms)[0]}`);
  window.currentFormId = Object.keys(Formio.forms)[0];

  if (!window.currentFormId || window.currentFormId == 0) {
    console.log("[IFRAME] No form id found yet... waiting...");
    console.log(Formio.forms);
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

window.isSubmitted = window.isSubmitted || false;
// Early return if form is already submitted
if (window.isSubmitted) return;

if (!window || !window.parent || !window.parent.postMessage || !data) return;

// Define a flag to track the event listener on the window object
window.listenerAdded = window.listenerAdded || {};

// Function to add a unique event listener to the window
function addUniqueWindowEventListener(event, listener) {
  // Check if the listener for the specific event has already been added
  if (!window.listenerAdded[event]) {
    console.log(
      `addUniqueWindowEventListener: listener for event: ${event} has not yet been added, adding...`
    );
    window.addEventListener(event, listener);
    window.listenerAdded[event] = true; // Update the flag for this event
  } else {
    console.log(
      `addUniqueWindowEventListener: listener for event: ${event} already added, do nothing.`
    );
  }
}

// Add the resize event listener to the window
addUniqueWindowEventListener('message', (event) => {
  console.log('iframe detected message event...');
  console.log(event);
});

function sendSubmissionToParent(status) {
  const message = {
    type: 'PUT_CHEFS_SUBMISSION',
    status,
    submission: data,
  };

  const messageObj = JSON.parse(JSON.stringify(message));

  if (window.cachedMessage === JSON.stringify(messageObj)) {
    console.log('No change in form data, skip posting message to parent...');
    return;
  }

  // Send the message to the parent window
  window.parent.postMessage(messageObj, 'https://localhost:4200/'); // Use a specific origin instead of '*' for better security
  window.cachedMessage = JSON.stringify(messageObj);
}

window.sendSubmissionToParent = sendSubmissionToParent;

// Call the function when you need to send the message and only when data is present
sendSubmissionToParent('Draft');

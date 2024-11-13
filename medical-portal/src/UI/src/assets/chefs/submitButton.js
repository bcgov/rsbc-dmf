// Early return if form is already submitted
if (window.isSubmitted) return;

if (
  !window ||
  !window.parent ||
  !window.parent.postMessage ||
  !window.putChefsSubmission ||
  !data
)
  return;

// Call the function when you need to send the message and only when data is present
if (confirm("Are you sure you want to submit the form?")) {
window.putChefsSubmission('Final');
  window.isSubmitted = true;
}

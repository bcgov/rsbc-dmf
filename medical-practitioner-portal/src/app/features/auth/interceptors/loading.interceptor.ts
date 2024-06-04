// import { inject } from '@angular/core';
// import {
//   HttpHandlerFn,
//   HttpInterceptorFn,
//   HttpRequest,
// } from '@angular/common/http';
// import { NgxSpinnerService } from 'ngx-spinner';
// import { finalize } from 'rxjs/operators';

// type HttpMethod = 'GET' | 'PUT' | 'POST' | 'DELETE' | 'HEAD';
// interface PortalRequest {
//   url: RegExp;
//   methods?: HttpMethod[];
// }

// const includedURLs: PortalRequest[] = [
//   {
//     url: /^\/api\/.+$/,
//   },
// ];

// const excludedURLs: PortalRequest[] = [];

// const matchRequest =
//   (request: HttpRequest<any>) =>
//   ({ url: regexp, methods }: PortalRequest) =>
//     regexp.test(request.url) &&
//     (!methods || methods?.some((method) => method === request.method));

// let requestsInProgressCount = 0;

// export const ApiLoaderInterceptor: HttpInterceptorFn = (
//   request: HttpRequest<any>,
//   next: HttpHandlerFn,
// ) => {
//   const spinnerService = inject(NgxSpinnerService);

//   const isIncluded = includedURLs.some(matchRequest(request));
//   const isExcluded = excludedURLs.some(matchRequest(request));

//   // Excluded URLs take precedence.  If the URL is in the excluded list or NOT in the included list
//   // then DO NOT show the loading spinner.
//   if (isExcluded || !isIncluded) {
//     return next(request);
//   }

//   requestsInProgressCount++;
//   spinnerService.show('apiLoadingSpinner');

//   return next(request).pipe(
//     finalize(() => {
//       requestsInProgressCount--;
//       if (requestsInProgressCount <= 0)
//         setTimeout(() => {
//           spinnerService.hide('apiLoadingSpinner');
//         }, 2000);
//     }),
//   );
// };

import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs/operators';

let pendingRequests = 0;

export const spinnerInterceptor: HttpInterceptorFn = (req, next) => {
  const spinner = inject(NgxSpinnerService);

  pendingRequests++;
  spinner.show('apiLoadingSpinner');

  return next(req).pipe(
    finalize(() => {
      pendingRequests = Math.max(0, pendingRequests - 1);
      if (pendingRequests === 0) {
        spinner.hide('apiLoadingSpinner');
      }
    })
  );
};

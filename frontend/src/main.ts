import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { provideToastr } from 'ngx-toastr';

provideToastr({
  timeOut: 3000,
  positionClass: 'toast-top-right',
  preventDuplicates: true,
  progressBar: true,
  tapToDismiss: false,
  closeButton: true
});


bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));

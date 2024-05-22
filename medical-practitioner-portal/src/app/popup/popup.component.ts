import {
  AfterViewInit,
  Component,
  ElementRef,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { PopupService } from './popup.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-popup',
  standalone: true,
  imports: [],
  templateUrl: './popup.component.html',
  styleUrl: './popup.component.scss',
})
export class PopupComponent implements AfterViewInit {
  @ViewChild('iframe') iframe!: ElementRef;
  public sanitizedSource!: SafeResourceUrl;
  iframeUrl: string =
    'https://submit.digital.gov.bc.ca/app/form/submit?f=5383fc89-b219-49a2-924c-251cd1557eb8';

  constructor(
    private popupService: PopupService,
    private sanitizer: DomSanitizer,
    private renderer: Renderer2
  ) {}

  getSourceURL(): SafeResourceUrl {
    return this.sanitizedSource;
  }

  ngOnInit() {
    this.sanitizedSource = this.sanitizer.bypassSecurityTrustResourceUrl(
      this.iframeUrl
    );
    // Listen for messages from iframe
    window.addEventListener('message', this.receiveMessage.bind(this), false);
  }

  ngAfterViewInit() {
    // const iframe = this.iframe.nativeElement;
    // // Add event listener to detect changes in iframe's URL
    // iframe.addEventListener('load', () => {
    //   const currentUrl = iframe.contentWindow.location.href;
    //   console.log('Current URL:', currentUrl);
    //   // You can perform any actions here based on the URL change
    // });
  }

  // addScriptToIframe() {
  //   const script = this.renderer.createElement('script');
  //   script.text = `
  //     window.addEventListener("message", receiveMessage, false);

  //     function receiveMessage(event) {
  //       window.parent.postMessage({ successMessagePresent: true }, event.origin);
  //     }
  //   `;
  //   if (this.iframe.nativeElement?.contentWindow) {
  //     console.log('appending iframe script');
  //     this.renderer.appendChild(
  //       this.iframe.nativeElement.contentWindow.document.body,
  //       script
  //     );
  //   }
  // }

  closePopup() {
    this.popupService.closePopup();
  }

  iframeLoaded() {
    console.log('Iframe loaded');
    console.log(this.iframe);
    if (this.iframe) {
      // Send message to iframe to check for success message
      console.log('posting message to iframe...');
      this.iframe.nativeElement.contentWindow.postMessage(
        'checkSuccessMessage',
        'https://submit.digital.gov.bc.ca'
      );
    }
  }

  receiveMessage(event: { data: { successMessagePresent: any } }) {
    console.log('received message from iframe....');
    console.log(event);
    if (this.iframe) {
      // Send message back to iframe to ack initial message
      console.log('acknowledging message from iframe...');
      this.iframe.nativeElement.contentWindow.postMessage(
        'ackMessage',
        'https://submit.digital.gov.bc.ca'
      );
    }
    // if (event.origin !== 'https://localhost:4200') return; // Ensure message is from expected origin
    // Handle messages from iframe
    if (event.data.successMessagePresent) {
      console.log('Success message found in iframe content');
      // You can perform any action you want here
    }
  }
}

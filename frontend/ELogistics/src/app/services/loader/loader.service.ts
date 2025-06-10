import { Injectable } from '@angular/core';
import { BehaviorSubject, interval, Subscription } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  private activeRequests = 0; // Counter for active requests
  private progressSubject = new BehaviorSubject<number>(0); // Observable for progress
  isLoading = this.progressSubject.asObservable(); // Expose progress observable
  private progressInterval!: Subscription;

  show() {
    this.activeRequests++; // Increment the active request counter

    if (this.activeRequests === 1) {
      // Only start the progress when the first request is made
      this.progressSubject.next(0); // Reset progress to 0%

      // Gradually increase progress to 90%
      this.progressInterval = interval(30).subscribe(() => {
        const currentProgress = this.progressSubject.getValue();
        if (currentProgress < 90) {
          this.progressSubject.next(currentProgress + 10);
        }
      });
    }
  }

  hide() {
    if (this.activeRequests > 0) {
      this.activeRequests--; // Decrement the active request counter
    }

    if (this.activeRequests === 0) {
      // When all requests are complete, set progress to 100% and reset
      this.progressSubject.next(100);

      if (this.progressInterval) {
        this.progressInterval.unsubscribe(); // Stop the progress interval
      }

      // Reset progress after a short delay
      setTimeout(() => {
        this.progressSubject.next(0);
      }, 100);
    }
  }
}

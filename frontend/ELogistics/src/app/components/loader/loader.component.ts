import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { LoaderService } from '../../services/loader/loader.service';

@Component({
  selector: 'app-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.css'
})
export class LoaderComponent {
  isLoading = false;
  progress = 0;

  constructor(private loaderService: LoaderService) {}

  ngOnInit(): void {
    // Subscribe to the loading state
    this.loaderService.isLoading.subscribe((value) => {
      this.isLoading = value > 0; // Show loader only if progress is greater than 0
      this.progress = value; // Update progress
    });
  }
}

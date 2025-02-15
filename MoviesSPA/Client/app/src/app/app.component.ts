import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <app-navigation-bar></app-navigation-bar>
    <router-outlet></router-outlet>
  `,
  standalone: false,
})
export class AppComponent {
  title = 'app';
}

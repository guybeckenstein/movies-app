import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MoviesFormComponent } from './components/movies-form/movies-form.component';
import { FavoritesComponent } from './components/favorites/favorites.component';

const routes: Routes = [
  { path: '', component: MoviesFormComponent }, // Default route
  { path: 'favorites', component: FavoritesComponent }, // Favorites page
  { path: '**', redirectTo: '', pathMatch: 'full' }, // Redirect unknown paths
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

import { Component } from '@angular/core';
import { Movie } from '../../data/movies.interface';

@Component({
  selector: 'app-favorites',
  standalone: false,
  templateUrl: './favorites.component.html',
})
export class FavoritesComponent {
  favoriteMovies: Movie[] = [];

  constructor() {
    this.loadFavorites();
  }

  // Load favorites from localStorage
  loadFavorites() {
    const storedFavorites = localStorage.getItem('favoriteMovies');
    if (storedFavorites) {
      this.favoriteMovies = JSON.parse(storedFavorites);
    }
  }

  // Remove a movie from favorites
  removeFavorite(movie: Movie) {
    this.favoriteMovies = this.favoriteMovies.filter(
      (fav) => fav.title !== movie.title
    );
    localStorage.setItem('favoriteMovies', JSON.stringify(this.favoriteMovies));
  }
}

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Movie } from '../../data/movies.interface';

@Component({
  selector: 'app-movie-card',
  standalone: false,
  templateUrl: './movie-card.component.html',
  styleUrl: './movie-card.component.scss',
})
export class MovieCardComponent {
  @Input({ required: true }) movie!: Movie;
  @Input({ required: true }) isFavorite = false;
  @Input() showButton = true;

  @Output() isFavoriteSelected = new EventEmitter();

  onToggleFavoriteButton() {
    this.isFavorite = !this.isFavorite;
    this.isFavoriteSelected.emit(this.movie);
  }
}

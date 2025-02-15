import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { Movie } from '../../data/movies.interface';
import { MoviesNetworkService } from '../../services/movies-network.service';
import { MatSelectChange } from '@angular/material/select';

@Component({
  selector: 'app-movies-form',
  standalone: false,
  templateUrl: './movies-form.component.html',
  styleUrl: './movies-form.component.scss',
})
export class MoviesFormComponent {
  // Filtering
  searchQueryControl: FormControl = new FormControl(''); // Reactive form control
  selectedGenre: string = '';
  selectedYear: string = '';

  movies: Movie[] = [];
  filteredMovies: Movie[] = [];
  favoriteMovies: Movie[] = [];
  genres: string[] = [];
  years: string[] = [];

  sortBy: 'title' | 'year' | 'titleDesc' | 'yearDesc' | '' = ''; // Sorting option

  displayErrorMessage = false;

  constructor(private moviesNetwork: MoviesNetworkService) {
    // this.onValueChanges();
  }

  private onValueChanges() {
    // Trigger search logic when input changes
    this.searchQueryControl.valueChanges
      .pipe(debounceTime(300))
      .subscribe((value) => {
        this.filterMovies(value);
      });
  }

  isFavoriteMovie(movie: Movie) {
    return (
      this.favoriteMovies.find((m) => m.title === movie.title) !== undefined
    );
  }

  ngOnInit() {
    // Fetch movies when the component is initialized
    this.moviesNetwork.GetMovies().subscribe({
      next: (movies) => {
        this.movies = movies;

        this.filterMovies(''); // Display filtered movies (when initialized - everything is displayed)
        this.genres = this.updateGenres(); // Initialize movies' genres array
        this.years = this.updateYears(); // Initialize movies' years array
        this.loadFavorites();
      },
      error: (err) => {
        console.error(err);
        this.displayErrorMessage = true;
      },
    });
  }

  isMovieInFilteredList(movie: Movie) {
    return this.filteredMovies.some((m) => m.title === movie.title);
  }

  onSearchButtonClick() {
    // Trigger search when search button is clicked
    this.filterMovies(this.searchQueryControl.value);
  }

  // Method to handle the 'Enter' key press
  onKeyDown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      // Trigger search when Enter key is pressed
      this.filterMovies(this.searchQueryControl.value);
    }
  }

  // Handler for sort change
  onSortChange(value: string) {
    this.sortBy = value as 'title' | 'year' | 'titleDesc' | 'yearDesc' | '';
    this.sortMovies();
  }

  // Method to handle search logic, by calling the movies API request
  private filterMovies(query: string) {
    let filtered = this.movies;

    // Apply search query filter
    if (query.trim()) {
      query = query.toLowerCase();
      filtered = filtered.filter((movie) =>
        movie.title.toLowerCase().includes(query)
      );
    }

    // Apply genre filter
    if (this.selectedGenre) {
      filtered = filtered.filter((movie) =>
        movie.genres.includes(this.selectedGenre)
      );
    }

    // Apply year filter
    if (this.selectedYear) {
      filtered = filtered.filter((movie) => movie.year === this.selectedYear);
    }

    this.filteredMovies = filtered;

    // Apply sorting after filtering
    this.sortMovies();
  }

  // Sorting function
  sortMovies() {
    if (this.sortBy === 'title') {
      this.filteredMovies.sort((a, b) => a.title.localeCompare(b.title));
    } else if (this.sortBy === 'year') {
      this.filteredMovies.sort(
        (a, b) =>
          Number(a.year.substring(0, 4)) - Number(b.year.substring(0, 4))
      );
    } else if (this.sortBy === 'titleDesc') {
      this.filteredMovies.sort((a, b) => b.title.localeCompare(a.title));
    } else if (this.sortBy === 'yearDesc') {
      this.filteredMovies.sort(
        (a, b) =>
          Number(b.year.substring(0, 4)) - Number(a.year.substring(0, 4))
      );
    }
  }

  private updateGenres() {
    let genres = new Set<string>();
    this.movies.forEach((movie) => {
      movie.genres.forEach((genre) => {
        genres.add(genre);
      });
    });

    return Array.from(genres.values());
  }

  private updateYears() {
    let years = new Set<string>();
    this.movies.forEach((movie) => {
      years.add(movie.year);
    });

    return Array.from(years.values());
  }

  updateFavorites(movie: Movie) {
    const index = this.favoriteMovies.findIndex(
      (fav) => fav.title === movie.title
    );
    if (index === -1) {
      this.favoriteMovies.push(movie);
    } else {
      this.favoriteMovies.splice(index, 1);
    }
    this.saveFavorites();
  }

  // Load favorites from localStorage
  loadFavorites() {
    const storedFavorites = localStorage.getItem('favoriteMovies');
    if (storedFavorites) {
      this.favoriteMovies = JSON.parse(storedFavorites);
    }
  }

  // Save favorites to localStorage
  saveFavorites() {
    localStorage.setItem('favoriteMovies', JSON.stringify(this.favoriteMovies));
  }
  // Filtering
  onGenreChange(value: string) {
    this.selectedGenre = value;
    this.filterMovies(this.searchQueryControl.value);
  }

  onYearChange(value: string) {
    this.selectedYear = value;
    this.filterMovies(this.searchQueryControl.value);
  }
}

import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { HttpClientService } from '../../shared/services/http-client.service';
import { AppConfig } from '../../shared/app-config';
import { HttpClient } from '@angular/common/http';
import { Movie } from '../data/movies.interface';

@Injectable({
  providedIn: 'root',
})
export class MoviesNetworkService extends HttpClientService {
  constructor(http: HttpClient) {
    super(http);
  }

  // Get movies
  GetMovies() {
    return this.get<Movie[]>(AppConfig.endpoints.movies).pipe(
      catchError(this.handleError)
    );
  }
}

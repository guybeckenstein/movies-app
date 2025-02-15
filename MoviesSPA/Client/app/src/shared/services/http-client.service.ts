import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AppConfig } from '../app-config';
import { getCookie } from '../utils/cookie-utils';

@Injectable({
  providedIn: 'root',
})
export class HttpClientService {
  protected apiBaseUrl = AppConfig.apiUrl;
  private authHeaders: HttpHeaders;

  constructor(private http: HttpClient) {
    this.authHeaders = this.createHeaders(); // Get the token and add to headers
  }

  // Helper method to create headers with Authorization (Bearer token from cookie)
  private createHeaders() {
    const token = getCookie(AppConfig.cookieKey);
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.append('Authorization', `Bearer ${token}`);
    }
    return headers;
  }

  // GET request
  protected get<T>(
    endpoint: string,
    params?: HttpParams,
    headers?: HttpHeaders
  ) {
    return this.http
      .get<T>(`${this.apiBaseUrl}/${endpoint}`, {
        headers: headers
          ? headers.append('Authorization', `Bearer ${getCookie('jwt')}`)
          : this.authHeaders,
        params: params,
      })
      .pipe(catchError(this.handleError));
  }

  // POST request with token from cookie
  protected post<T>(endpoint: string, body: any, headers?: HttpHeaders) {
    return this.http
      .post<T>(`${this.apiBaseUrl}/${endpoint}`, body, {
        headers: headers
          ? headers.append('Authorization', `Bearer ${getCookie('jwt')}`)
          : this.authHeaders,
      })
      .pipe(catchError(this.handleError));
  }

  // Error handling
  protected handleError(error: any): Observable<never> {
    throw new Error('Something went wrong with the request', error);
  }
}

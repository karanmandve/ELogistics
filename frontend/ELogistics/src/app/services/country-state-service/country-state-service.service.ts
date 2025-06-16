import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CountryStateServiceService {

  http = inject(HttpClient)


  getAllCountry(){
    return this.http.get("https://localhost:7228/api/CountryState/get-all-country");
  }

  getStateByCountryId(countryId: number){
    return this.http.get(`https://localhost:7228/api/CountryState/state-by-countryId/${countryId}`);
  }

  getAllState(){
    return this.http.get("https://localhost:7228/api/State/get-all-state");
  }


}

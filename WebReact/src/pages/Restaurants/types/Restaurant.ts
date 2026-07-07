export interface Restaurant {
  id: string;
  name: string;
  cuisine: string;
  borough: string;
}

export interface RestaurantInput {
  name: string;
  cuisine: string;
  borough: string;
}
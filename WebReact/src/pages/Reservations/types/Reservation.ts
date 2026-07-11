export interface Reservation {
  id: string;
  restaurantId: string;
  restaurantName: string | null;
  date: string;
}

export interface ReservationInput {
  restaurantId: string;
  date: string;
}
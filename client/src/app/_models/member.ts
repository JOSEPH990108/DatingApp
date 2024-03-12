import { Photo } from "./photo";

export interface Member {
  id:          number;
  userName:    string;
  age:         number;
  photoUrl:    string;
  knownAs:     string;
  created:     Date;
  lastActive:  Date;
  gender:      string;
  bio:         string;
  description: string;
  lookingFor:  string;
  interests:   string;
  city:        string;
  country:     string;
  photos:      Photo[];
}

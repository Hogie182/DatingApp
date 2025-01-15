import { User } from "./users";

export class UserParams {
    gender: string;
    minAge: number = 18;
    maxAge: number = 99;
    pageNumber: number = 1;
    pageSize: number = 5;
    orderBy: string = 'lastActive';

    constructor(user: User | null) {
        this.gender = user?.gender === 'female' ? 'male' : 'female'
    }
}
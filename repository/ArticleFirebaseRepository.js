import { IRepository } from './IRepository.js';
import { Article } from '../domain/Article.js'

const DB_BASE_URL = 'https://blog-4n-default-rtdb.europe-west1.firebasedatabase.app';

export class ArticleFirebaseRepository extends IRepository {
    async getAll() {
        //fect = richiesta a un indirizzo che ottiene ed elabora una risposta
        //fetch = chiamata asincrona
        //res = risposta dell'interrogazione
        const res = await fetch(`${DB_BASE_URL}/articles.json`);
        //controllo se l'interrogazione è andata a buon fine
        if (!res.ok) throw new Error(`HTTP error ${res.status}`);

        const data = await res.json();
        if (!data) return []; //controllo se il json è vuoto
        return Object.values(data).map(raw => new Article(raw));
    }

    async getById(id) {
        const res = await fetch(`${DB_BASE_URL}/articles/${id}.json`);
        if (!res.ok) throw new Error(`HTTP error ${res.status}`);

        const data = await res.json();
        if (!data) throw new Error(`Article with id "${id}" not found idiot`);
        return new Article(data);
    }
}
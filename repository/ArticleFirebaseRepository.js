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

    /*
    return Object.values(data).map(raw => new Article(raw));
    */

    return Object.keys(data).map(id => {
      const item = data[id];
      const timestamp = item.Timestamp ?? item.TimeStamp ?? item.timestamp;
      return new Article({ Id: id, Title: item.Title ?? '', Content: item.Content ?? '', Timestamp: timestamp });
    });
  }

  async getById(id) {
    const res = await fetch(`${DB_BASE_URL}/articles/${id}.json`);
    if (!res.ok) throw new Error(`HTTP error ${res.status}`);

    const data = await res.json();
    if (!data) throw new Error(`Article with id "${id}" not found`);
    const timestamp = data.Timestamp ?? data.TimeStamp ?? data.timestamp;
    return new Article({ Id: id, Title: data.Title ?? '', Content: data.Content ?? '', Timestamp: timestamp });
  }

  async save(article) {
    const res = await fetch(`${DB_BASE_URL}/articles/${article.Id}.json`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(article.toJSON()),
    });
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);
  }

  async delete(id) {
    const res = await fetch(`${DB_BASE_URL}/articles/${id}.json`, {
      method: 'DELETE',
    });
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);
  }

  async update(article) {
    const res = await fetch(`${DB_BASE_URL}/articles/${article.Id}.json`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(article.toJSON())
    }); 
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);
  }
}
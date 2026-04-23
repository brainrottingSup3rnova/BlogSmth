import { Article } from '../domain/Article.js';

export class ArticleService {
    constructor(repository) {
        this._repository = repository;
    }

    async getAll() {
        const articles = await this._repository.getAll();
        return articles.sort((a, b) => (b.Timestamp ?? 0) - (a.Timestamp ?? 0));
    }

    async getById(id) {
        if (!id.trim()) throw new Error('Id invalid');
        return this._repository.getById(id);
    }

    async save(article) {     
        if (!article.Title.trim()) throw new Error('Title invalid, just like you!');
        if (!article.Content.trim()) throw new Error('Content invalid, just like you!');
        const articleToSave = new Article({
            Id: Article.generateId(),
            Title: article.Title,
            Content: article.Content,
            Timestamp: Math.floor(Date.now() / 1000)
        });
        return this._repository.save(articleToSave);
    }
    
    async delete(id) {
        if(!id?.trim()) throw new Error('Invalid id, just like you!');
        return this._repository.delete(id);
    }

    async update(id, article) {
        if (!article.Title.trim()) throw new Error('Title invalid, just like you!');
        if (!article.Content.trim()) throw new Error('Content invalid, just like you!');
        if(!id?.trim()) throw new Error('Invalid id, just like you!');
        const articleToSave = new Article({
            Id: id,
            Title: article.Title,
            Content: article.Content,
            Timestamp: Math.floor(Date.now() / 1000)
        });
        return this._repository.update(articleToSave);
    }
}


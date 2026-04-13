export class Article {
    constructor({Id = null, Title='', Content='', Timestamp = null} = { }){
        this.Id = Id;
        this.Title = Title;
        this.Content = Content;
        this.Timestamp = Timestamp;
    }

    formattedDate() {
        if(!this.Timestamp) return '';

        return new Date(this.Timestamp * 1000).toLocaleDateString('it-IT', {
            day: '2-digit', month: 'long', year: 'numeric',
        });
    }

    toJSON() {
        return {
            Id: this.Id,
            Title: this.Title,
            Content: this.Content,
            Timestamp: this.Timestamp,

        };
    }
}
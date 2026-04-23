//classe usata come interfaccia perchè le interfaccie su js non esistono
export class IRepository {

    //i metodi async su js restituiscono una promise (equivalente di un task)
    async getAll() {
        throw new Error(`${this.constructor.name} needs to implement getAll`);
    }

    async getById(id) {
        throw new Error(`${this.constructor.name} needs to implement getById`);
    }

    async save(entity) {
        throw new Error(`${this.constructor.name} needs to implement save`);
    }

    async delete(id) {
        throw new Error(`${this.constructor.name} needs to implement delete`);
    }
    
    async update(id,article) {
        throw new Error(`${this.constructor.name} needs to implement update`);
    }
}
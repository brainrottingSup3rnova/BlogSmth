//classe usata come interfaccia perchè le interfaccie su js non esistono
export class IRepository {

    //i metodi async su js restituiscono una promise (equivalente di un task)
    async getAll() {
        throw new Error(`${this.constructor.name} deve implementare getAll`);
    }

    async getById(id) {
        throw new Error(`${this.constructor.name} deve implementare getById`);
    }

    async save(entity) {
        throw new Error(`${this.constructor.name} deve implementare save`);
    }

    async delete(id) {
        throw new Error(`${this.constructor.name} deve implementare delete`);
    }
}
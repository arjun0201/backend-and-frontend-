import Product from './Product'
export interface ProductColumnDefinition {
    label: string,
    key: keyof Product,
    value: Product[keyof Product],
    isId?: boolean,
    type: string,
}

export const productColumnDefinitions : ProductColumnDefinition[] = [
    { label: 'ID', key: 'id', value: '', type: 'text', isId: true},
    { label: 'Name', key: 'name', value: '', type: 'text'},
    { label: 'Brand', key: 'brand', value: '', type: 'text'},
    { label: 'Price(EUR)', key: 'price', value: 0, type: 'number'},
    { label: 'Description', key: 'description', value: '', type: 'text'},
    { label: 'Stock', key: 'stock', value: 0, type: 'number'},
]
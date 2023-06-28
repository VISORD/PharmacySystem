export const DRAFT = { id: 0, name: 'Draft' }
export const ORDERED = { id: 1, name: 'Ordered' }
export const SHIPPED = { id: 2, name: 'Shipped' }
export const DELIVERED = { id: 3, name: 'Delivered' }

export const allOrderStatuses = [DRAFT, ORDERED, SHIPPED, DELIVERED]

export function resolveOrderStatus(value) {
    for (const item of allOrderStatuses) {
        if (item.id === value) {
            return item.name
        }
    }

    return null
}

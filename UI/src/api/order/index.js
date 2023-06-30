import api from '@/api'

export function list({ filtering, ordering, paging }) {
    return api.post(
        '/api/order/list',
        {
            filtering: filtering,
            ordering: ordering,
            paging: paging
        },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function add({ pharmacyId }) {
    return api.post(
        '/api/order',
        {
            pharmacyId: pharmacyId
        },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function get(orderId) {
    return api.get(`/api/order/${orderId}`)
}

export function remove(orderId) {
    return api.delete(`/api/order/${orderId}`)
}

export function launch(orderId) {
    return api.put(`/api/order/${orderId}/launch`)
}

export function ship(orderId) {
    return api.put(`/api/order/${orderId}/ship`)
}

export function complete(orderId) {
    return api.put(`/api/order/${orderId}/complete`)
}

export function history(orderId) {
    return api.get(`/api/order/${orderId}/history`)
}

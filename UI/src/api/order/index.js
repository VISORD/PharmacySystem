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

export function get(orderId) {
    return api.get(`/api/order/${orderId}`)
}

export function remove(orderId) {
    return api.delete(`/api/order/${orderId}`)
}

export function history(orderId) {
    return api.get(`/api/order/${orderId}/history`)
}

import api from '@/api'

export function list(orderId, { filtering, ordering, paging }) {
    return api.post(
        `/api/order/${orderId}/medicament/list`,
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

export function request(orderId, medicamentId, { count }) {
    return api.put(
        `/api/order/${orderId}/medicament/${medicamentId}/request`,
        { count: count },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function approve(orderId, medicamentId, { count }) {
    return api.put(
        `/api/order/${orderId}/medicament/${medicamentId}/approve`,
        { count: count },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function disapprove(orderId, medicamentId) {
    return api.put(`/api/order/${orderId}/medicament/${medicamentId}/disapprove`)
}

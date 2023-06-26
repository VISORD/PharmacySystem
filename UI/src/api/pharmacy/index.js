import api from '@/api'

export function list({ filtering, ordering, paging }) {
    return api.post(
        '/api/pharmacy/list',
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

export function get(pharmacyId) {
    return api.get(`/api/pharmacy/${pharmacyId}`)
}

export function remove(pharmacyId) {
    return api.delete(`/api/pharmacy/${pharmacyId}`)
}

import api from '@/api'

export function list({ filtering, ordering, paging }) {
    return api.post(
        '/api/medicament/list',
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

export function get(medicamentId) {
    return api.get(`/api/medicament/${medicamentId}`)
}

export function remove(medicamentId) {
    return api.delete(`/api/medicament/${medicamentId}`)
}

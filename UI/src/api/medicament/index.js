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

export function analogues(medicamentId, { filtering, ordering, paging }) {
    return api.post(
        `/api/medicament/${medicamentId}/analogue/list`,
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

export function associate(medicamentId, analogueIds) {
    return api.put(`/api/medicament/${medicamentId}/analogue/associate`, analogueIds, {
        headers: {
            'Content-Type': 'application/json'
        }
    })
}

export function disassociate(medicamentId, analogueIds) {
    return api.put(`/api/medicament/${medicamentId}/analogue/disassociate`, analogueIds, {
        headers: {
            'Content-Type': 'application/json'
        }
    })
}

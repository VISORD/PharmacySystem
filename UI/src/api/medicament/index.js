import api from '@/api'

export function list({ filtering, ordering, paging, excludeById = undefined }) {
    return api.post(
        '/api/medicament/list',
        {
            filtering: filtering,
            ordering: ordering,
            paging: paging,
            excludeById: excludeById
        },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function add({ name, vendorPrice, description }) {
    return api.post(
        '/api/medicament',
        {
            name: name,
            vendorPrice: vendorPrice,
            description: description || null
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

export function update(medicamentId, { name, vendorPrice, description }) {
    return api.put(
        `/api/medicament/${medicamentId}`,
        {
            name: name,
            vendorPrice: vendorPrice,
            description: description || null
        },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function remove(medicamentId) {
    return api.delete(`/api/medicament/${medicamentId}`)
}

export function analogueList(medicamentId, { filtering, ordering, paging }) {
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

import api from '@/api'

export function list(pharmacyId, { filtering, ordering, paging }) {
    return api.post(
        `/api/pharmacy/${pharmacyId}/medicament/list`,
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

export function get(pharmacyId, medicamentId) {
    return api.get(`/api/pharmacy/${pharmacyId}/medicament/${medicamentId}`)
}

export function rateList(pharmacyId, medicamentId, { filtering, ordering, paging }) {
    return api.post(
        `/api/pharmacy/${pharmacyId}/medicament/${medicamentId}/rate/list`,
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

export function saleList(pharmacyId, medicamentId, { filtering, ordering, paging }) {
    return api.post(
        `/api/pharmacy/${pharmacyId}/medicament/${medicamentId}/sale/list`,
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

export function orderList(pharmacyId, medicamentId, { filtering, ordering, paging }) {
    return api.post(
        `/api/pharmacy/${pharmacyId}/medicament/${medicamentId}/order/list`,
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

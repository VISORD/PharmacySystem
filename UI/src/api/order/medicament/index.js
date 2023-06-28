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

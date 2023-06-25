import api from '@/api'

export function list({ paging, ordering, filtering }) {
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

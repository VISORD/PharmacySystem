import api from '@/api'

export function list({ paging, ordering, filtering }) {
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

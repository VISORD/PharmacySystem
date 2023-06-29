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

export function add({ name, email, phone, address, description, latitude, longitude, workingHours }) {
    return api.post(
        '/api/pharmacy',
        {
            name: name,
            email: email || null,
            phone: phone || null,
            address: address,
            description: description || null,
            latitude: latitude,
            longitude: longitude,
            workingHours: workingHours
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

export function update(pharmacyId, { name, email, phone, address, description, latitude, longitude, workingHours }) {
    return api.put(
        `/api/pharmacy/${pharmacyId}`,
        {
            name: name,
            email: email || null,
            phone: phone || null,
            address: address,
            description: description || null,
            latitude: latitude,
            longitude: longitude,
            workingHours: workingHours
        },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function remove(pharmacyId) {
    return api.delete(`/api/pharmacy/${pharmacyId}`)
}

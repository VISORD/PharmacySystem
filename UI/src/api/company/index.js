import api from '@/api'

export function get() {
    return api.get('/api/company')
}

export function update({ email, name, phone }) {
    return api.put(
        '/api/company',
        {
            email: email,
            name: name,
            phone: phone || null
        },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

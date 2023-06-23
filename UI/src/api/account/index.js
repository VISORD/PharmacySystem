import api from '@/api'

export function signIn({ email, password }) {
    return api.post(
        '/api/account/sign-in',
        { email: email, password: password },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function signUp({ email, name, password }) {
    return api.post(
        '/api/account/sign-up',
        { email: email, name: name, password: password },
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )
}

export function signOut() {
    return api.post('/api/account/sign-out')
}

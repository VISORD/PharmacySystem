import axios from 'axios'
import { useAccountStore } from '@/stores/account'

const client = axios.create({
    baseURL: import.meta.env.VITE_API_SERVER_URL,
    withCredentials: true,
    headers: {
        accept: '*/*',
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        Expires: '0'
    }
})

client.interceptors.response.use(undefined, async (error) => {
    if (error.response.status === 401) {
        const account = useAccountStore()
        await account.onUnauthorized()
    }

    return error.response
})

export default client

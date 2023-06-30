import { defineStore } from 'pinia'
import { signIn, signOut, signUp } from '@/api/account'
import { readonly, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import router from '@/plugins/router'

const accessible = 'accessible'

const userSignedIn = 'User signed in'
const welcomeBack = 'Welcome back!'

const userSignedUp = 'User signed up'
const welcome = 'Welcome to the Pharmacy System!'

const userSignedOut = 'User signed out'
const sessionWasClosed = 'Session was closed'
const sessionHasExpired = 'Session has expired'

const authenticationFailed = 'Authentication failed'

export const useAccountStore = defineStore('account', () => {
    const isAuthenticated = ref(localStorage.getItem(accessible) === 'true')
    const toast = useToast()

    const processing = ref(false)

    async function setAsAuthenticated() {
        localStorage.setItem(accessible, 'true')
        isAuthenticated.value = true

        const redirect = router.currentRoute.value.query['redirect']
        await router.push(redirect ? redirect : '/')
    }

    async function setAsUnauthenticated(redirect = null) {
        localStorage.removeItem(accessible)
        isAuthenticated.value = false

        let link = '/auth'
        if (redirect !== null && redirect !== '/') {
            link += '?redirect=' + redirect
        }

        await router.push(link)
    }

    async function trySignIn({ email, password }) {
        processing.value = true

        const response = await signIn({ email, password })

        let notification
        if (response.status < 400) {
            await setAsAuthenticated()
            notification = {
                severity: 'success',
                summary: userSignedIn,
                detail: welcomeBack,
                life: 3000
            }
        } else if (response.status !== 401) {
            notification = {
                severity: 'error',
                summary: authenticationFailed,
                detail: response.data.error,
                life: 3000
            }
        }

        if (notification) {
            toast.add(notification)
        }

        processing.value = false
    }

    async function trySignUp({ email, name, password }) {
        processing.value = true

        const response = await signUp({ email, name, password })

        let notification
        if (response.status < 400) {
            await setAsAuthenticated()
            notification = {
                severity: 'success',
                summary: userSignedUp,
                detail: welcome,
                life: 3000
            }
        } else if (response.status !== 401) {
            notification = {
                severity: 'error',
                summary: authenticationFailed,
                detail: response.data.error,
                life: 3000
            }
        }

        if (notification) {
            toast.add(notification)
        }

        processing.value = false
    }

    async function trySignOut() {
        processing.value = true

        const response = await signOut()

        let notification
        if (response.status < 400) {
            await setAsUnauthenticated()

            notification = {
                severity: 'success',
                summary: userSignedOut,
                detail: sessionWasClosed,
                life: 3000
            }
        } else if (response.status !== 401) {
            notification = {
                severity: 'error',
                summary: userSignedOut,
                detail: response.data.error,
                life: 3000
            }
        }

        if (notification) {
            toast.add(notification)
        }

        processing.value = false
    }

    async function onUnauthorized() {
        await setAsUnauthenticated(router.currentRoute.value.fullPath)
        toast.add({
            severity: 'warn',
            summary: userSignedOut,
            detail: sessionHasExpired,
            life: 3000
        })
    }

    return {
        isAuthenticated: readonly(isAuthenticated),
        processing,
        trySignIn,
        trySignUp,
        trySignOut,
        onUnauthorized
    }
})

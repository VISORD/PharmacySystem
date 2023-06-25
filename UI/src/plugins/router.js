import { createRouter, createWebHistory } from 'vue-router'
import { useAccountStore } from '@/stores/account'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'content',
            component: () => import('@/views/ContentView.vue'),
            beforeEnter: (to, from, next) => {
                const account = useAccountStore()
                if (!account.isAuthenticated) {
                    if (to.fullPath !== '/') {
                        next({ path: '/auth', query: { redirect: to.fullPath } })
                    } else {
                        next({ path: '/auth' })
                    }
                } else next()
            },
            children: [
                {
                    path: '',
                    name: 'company',
                    component: () => import('@/views/CompanyView.vue')
                },
                {
                    path: 'pharmacy',
                    name: 'pharmacies',
                    component: () => import('@/views/PharmaciesView.vue')
                },
                {
                    path: 'medicament',
                    name: 'medicaments',
                    component: () => import('@/views/MedicamentsView.vue')
                },
                {
                    path: 'order',
                    name: 'orders',
                    component: () => import('@/views/OrdersView.vue')
                }
            ]
        },
        {
            path: '/auth',
            name: 'authentication',
            component: () => import('@/views/AuthenticationView.vue'),
            beforeEnter: (to, from, next) => {
                const account = useAccountStore()
                if (account.isAuthenticated) {
                    next({ path: from.fullPath })
                } else next()
            }
        }
    ]
})

export default router

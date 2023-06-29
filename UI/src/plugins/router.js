import { createRouter, createWebHistory } from 'vue-router'
import { useAccountStore } from '@/stores/account'
import { useCompanyStore } from '@/stores/company'
import { usePharmacyStore } from '@/stores/pharmacy'
import { useMedicamentStore } from '@/stores/medicament'
import { useOrderStore } from '@/stores/order'
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'

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
                    component: () => import('@/views/CompanyView.vue'),
                    beforeEnter: (to, from, next) => {
                        const company = useCompanyStore()
                        company.dialog = !!to.query.companyEditForm

                        next()
                    }
                },
                {
                    path: 'pharmacy',
                    name: 'pharmacies',
                    component: () => import('@/views/PharmaciesView.vue'),
                    beforeEnter: (to, from, next) => {
                        const pharmacy = usePharmacyStore()
                        pharmacy.view.dialog = !!to.query.pharmacyId
                        pharmacy.view.pharmacyId = to.query.pharmacyId
                        pharmacy.edit.pending = !!to.query.pharmacyEditForm

                        const pharmacyMedicament = usePharmacyMedicamentStore()
                        pharmacyMedicament.view.dialog = !!to.query.medicamentId
                        pharmacyMedicament.view.medicamentId = to.query.medicamentId

                        next()
                    }
                },
                {
                    path: 'medicament',
                    name: 'medicaments',
                    component: () => import('@/views/MedicamentsView.vue'),
                    beforeEnter: (to, from, next) => {
                        const medicament = useMedicamentStore()
                        medicament.view.dialog = !!to.query.medicamentId
                        medicament.view.medicamentId = to.query.medicamentId
                        medicament.edit.pending = !!to.query.medicamentEditForm

                        next()
                    }
                },
                {
                    path: 'order',
                    name: 'orders',
                    component: () => import('@/views/OrdersView.vue'),
                    beforeEnter: (to, from, next) => {
                        const order = useOrderStore()
                        order.view.dialog = !!to.query.orderId
                        order.view.orderId = to.query.orderId

                        next()
                    }
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

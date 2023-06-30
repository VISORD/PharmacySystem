import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { usePharmacyStore } from '@/stores/pharmacy'
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'
import { orderList } from '@/api/pharmacy/medicament'
import router from '@/plugins/router'

const columns = {
    orderId: {
        key: 'orderId',
        field: 'OrderId',
        header: 'Order #',
        matchMode: FilterMatchMode.EQUALS
    },
    orderCount: {
        key: 'orderCount',
        field: 'OrderCount',
        header: 'Order Count',
        matchMode: FilterMatchMode.EQUALS
    },
    status: {
        key: 'status',
        field: 'StatusId',
        header: 'Status',
        matchMode: FilterMatchMode.IN
    },
    orderedAt: {
        key: 'orderedAt',
        field: 'OrderedAt',
        header: 'Ordered At',
        matchMode: FilterMatchMode.DATE_IS
    },
    updatedAt: {
        key: 'updatedAt',
        field: 'UpdatedAt',
        header: 'Updated At',
        matchMode: FilterMatchMode.DATE_IS
    }
}

export const usePharmacyMedicamentOrderStore = defineStore('pharmacy-medicament-order', () => {
    const toast = useToast()

    const pharmacy = usePharmacyStore()
    const pharmacyMedicament = usePharmacyMedicamentStore()

    const table = ref({
        loading: true,
        columns: columns,
        data: {
            items: [],
            totalAmount: 0
        },
        selection: null,
        filtering: defaultFiltering(columns),
        ordering: defaultOrdering(),
        paging: defaultPaging(),
        async reload({
            filters = undefined,
            orders = undefined,
            pageFirst = undefined,
            pageNumber = undefined,
            pageSize = undefined
        } = {}) {
            this.loading = true

            const request = preparePagingRequest(this, { filters, orders, pageFirst, pageNumber, pageSize })
            const response = await orderList(pharmacy.view.pharmacyId, pharmacyMedicament.view.medicamentId, request)

            if (response.status < 400) {
                this.data = response.data
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy medicament orders load failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.loading = false
        },
        async reset() {
            this.selection = null
            this.filtering = defaultFiltering(columns)
            this.ordering = defaultOrdering()
            this.paging = defaultPaging()
            await this.reload()
        },
        selectRow(selection) {
            this.selection = selection
        },
        selectForContextMenu(selection) {
            this.selection = selection
        },
        showInfo() {
            if (!this.selection) {
                return
            }

            window.open(
                router.resolve({
                    path: 'order',
                    query: { orderId: this.selection.orderId }
                }).href,
                '_blank'
            )
        }
    })

    return { table }
})

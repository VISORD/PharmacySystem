import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { list } from '@/api/order/medicament'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { useOrderStore } from '@/stores/order'
import router from '@/plugins/router'

const columns = {
    medicament: {
        key: 'medicament',
        field: 'MedicamentId',
        header: 'Medicament',
        matchMode: FilterMatchMode.EQUALS
    },
    quantityOnHand: {
        key: 'quantityOnHand',
        field: 'QuantityOnHand',
        header: 'Quantity on Hand',
        matchMode: FilterMatchMode.EQUALS
    },
    requestedCount: {
        key: 'requestedCount',
        field: 'RequestedCount',
        header: 'Requested Count',
        matchMode: FilterMatchMode.EQUALS
    },
    approvedCount: {
        key: 'approvedCount',
        field: 'ApprovedCount',
        header: 'Approved Count',
        matchMode: FilterMatchMode.EQUALS
    },
    isApproved: {
        key: 'isApproved',
        field: 'IsApproved',
        header: 'Is Approved?',
        matchMode: FilterMatchMode.IN
    }
}

export const useOrderMedicamentStore = defineStore('order-medicament', () => {
    const toast = useToast()

    const order = useOrderStore()

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
            const response = await list(order.view.orderId, request)

            if (response.status < 400) {
                this.data = response.data
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order medicaments load failed',
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
                    path: 'medicament',
                    query: { medicamentId: this.selection.medicament.id }
                }).href,
                '_blank'
            )
        }
    })

    return { table }
})

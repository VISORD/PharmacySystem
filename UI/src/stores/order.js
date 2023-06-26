import { defineStore } from 'pinia'
import { ref } from 'vue'
import { FilterMatchMode } from 'primevue/api'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { get, list, remove } from '@/api/order'

const columns = {
    id: {
        key: 'id',
        field: 'Id',
        header: 'Order #',
        matchMode: FilterMatchMode.EQUALS
    },
    pharmacy: {
        key: 'pharmacy',
        field: 'PharmacyId',
        header: 'Pharmacy',
        matchMode: FilterMatchMode.EQUALS
    },
    medicamentItemCount: {
        key: 'medicamentItemCount',
        field: 'MedicamentItemCount',
        header: 'Medicament Item Count',
        matchMode: FilterMatchMode.EQUALS
    },
    status: {
        key: 'status',
        field: 'Status',
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

export const useOrderStore = defineStore('order', () => {
    const toast = useToast()

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
            this.selection = null

            const request = preparePagingRequest(this, { filters, orders, pageFirst, pageNumber, pageSize })
            const response = await list(request)

            if (response.status < 400) {
                this.data = response.data
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Orders load failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.loading = false
        },
        async reset() {
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
        async showInfo() {
            if (!this.selection?.id) {
                return
            }

            view.value.dialog = true
            await view.value.reload(this.selection.id)
        },
        async tryDelete() {
            if (!this.selection?.id) {
                return
            }

            const response = await remove(this.selection.id)
            if (response.status < 400 && response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order delete failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            view.value.dialog = false

            this.paging = defaultPaging()
            await this.reload()
        }
    })

    const view = ref({
        dialog: false,
        loading: true,
        profile: {},
        async reload(orderId) {
            this.loading = true

            const response = await get(orderId)
            if (response.status < 400) {
                this.profile = response.data.item
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order info getting failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.loading = false
        }
    })

    return { table, view }
})

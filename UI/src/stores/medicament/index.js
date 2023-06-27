import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { get, list, remove } from '@/api/medicament'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'

const columns = {
    name: {
        key: 'name',
        field: 'Name',
        header: 'Name',
        matchMode: FilterMatchMode.EQUALS
    },
    vendorPrice: {
        key: 'vendorPrice',
        field: 'VendorPrice',
        header: 'Vendor Price',
        matchMode: FilterMatchMode.EQUALS
    }
}

export const useMedicamentStore = defineStore('medicament', () => {
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
                    summary: 'Medicaments load failed',
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
            view.value.medicamentId = this.selection.id
        },
        async tryDelete() {
            if (!this.selection) {
                return
            }

            const response = await remove(this.selection.id)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Medicament deleted',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament delete failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.paging = defaultPaging()
            await this.reload()
        }
    })

    const view = ref({
        medicamentId: null,
        dialog: false,
        loading: true,
        profile: {},
        async reload() {
            this.loading = true

            const response = await get(this.medicamentId)
            if (response.status < 400) {
                this.profile = response.data.item
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament info getting failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.loading = false
        },
        async tryDelete() {
            if (!this.medicamentId) {
                return
            }

            const response = await remove(this.medicamentId)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Medicament deleted',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament delete failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.dialog = false
            this.medicamentId = null
        }
    })

    return { table, view }
})

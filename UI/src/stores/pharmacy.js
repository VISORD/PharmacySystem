import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { get, list, remove } from '@/api/pharmacy'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { FilterMatchMode } from 'primevue/api'
import { preparePagingRequest } from '@/utils/paging'

const columns = {
    name: {
        key: 'name',
        field: 'Name',
        header: 'Name',
        matchMode: FilterMatchMode.EQUALS
    },
    address: {
        key: 'address',
        field: 'Address',
        header: 'Address',
        matchMode: FilterMatchMode.EQUALS
    }
}

export const usePharmacyStore = defineStore('pharmacy', () => {
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
                    summary: 'Pharmacies load failed',
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
                    summary: 'Pharmacy delete failed',
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
        async reload(pharmacyId) {
            this.loading = true

            const response = await get(pharmacyId)
            if (response.status < 400) {
                this.profile = response.data.item
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy info getting failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.loading = false
        }
    })

    return { table, view }
})

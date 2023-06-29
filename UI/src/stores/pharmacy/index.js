import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { add, get, list, remove, update } from '@/api/pharmacy'
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

            if (edit.value.pending && !view.value.dialog) {
                edit.value.pending = false
                edit.value.dialog = true
            }
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
        showInfo() {
            if (!this.selection) {
                return
            }

            view.value.dialog = true
            view.value.pharmacyId = this.selection.id
        },
        async tryDelete() {
            if (!this.selection?.id) {
                return
            }

            const response = await remove(this.selection.id)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Pharmacy deleted',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy delete failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.paging = defaultPaging()
            await this.reload()
        }
    })

    const view = ref({
        pharmacyId: null,
        dialog: false,
        loading: true,
        profile: {},
        close() {
            this.pharmacyId = null
            this.profile = {}
            edit.value.pending = false
        },
        async reload() {
            this.loading = true

            const response = await get(this.pharmacyId)
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

            if (edit.value.pending) {
                edit.value.pending = false
                edit.value.dialog = true
            }
        },
        async tryDelete() {
            if (!this.pharmacyId) {
                return
            }

            const response = await remove(this.pharmacyId)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Pharmacy deleted',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })

                table.value.paging = defaultPaging()
                await table.value.reload()
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy delete failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.dialog = false
            this.close()
        }
    })

    const edit = ref({
        dialog: false,
        pending: false,
        async tryApply(values) {
            const response = view.value.pharmacyId ? await update(view.value.pharmacyId, values) : await add(values)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: view.value.pharmacyId ? 'Pharmacy info updated' : 'New pharmacy added',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })

                this.dialog = false

                if (view.value.pharmacyId) {
                    await view.value.reload()
                }

                table.value.paging = defaultPaging()
                await table.value.reload()
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: view.value.pharmacyId ? 'Pharmacy info update failed' : 'New pharmacy addition failed',
                    detail: response.data.error,
                    life: 3000
                })
            }
        }
    })

    return { table, view, edit }
})

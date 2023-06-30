import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { add, get, list, remove, update } from '@/api/medicament'
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

            if (edit.value.pending && !view.value.dialog) {
                edit.value.pending = false
                edit.value.dialog = true
            }
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
            if (!this.selection?.id) {
                return
            }

            view.value.dialog = true
            view.value.medicamentId = this.selection.id
        },
        doubleClick() {
            this.showInfo()
        },
        async tryDelete() {
            if (!this.selection) {
                return
            }

            const response = await remove(this.selection.id)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Medicament has deleted',
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

            this.selection = null
            this.paging = defaultPaging()
            await this.reload()
        }
    })

    const view = ref({
        medicamentId: null,
        dialog: false,
        loading: true,
        profile: {},
        close() {
            this.medicamentId = null
            this.profile = {}
            edit.value.pending = false
        },
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

            if (edit.value.pending) {
                edit.value.pending = false
                edit.value.dialog = true
            }
        },
        async tryDelete() {
            if (!this.medicamentId) {
                return
            }

            const response = await remove(this.medicamentId)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Medicament has deleted',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })

                table.value.selection = null
                table.value.paging = defaultPaging()
                await table.value.reload()
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament delete failed',
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
        processing: false,
        async tryApply(values) {
            this.processing = true

            const response = view.value.medicamentId ? await update(view.value.medicamentId, values) : await add(values)
            if (response.status < 400) {
                this.dialog = false
                setTimeout(async () => {
                    if (view.value.medicamentId) {
                        await view.value.reload()
                    } else {
                        view.value.medicamentId = response.data.item.id
                        view.value.dialog = true
                    }

                    setTimeout(
                        () =>
                            toast.add({
                                severity: 'success',
                                summary: view.value.medicamentId
                                    ? 'Medicament info has updated'
                                    : 'New medicament has added',
                                detail: 'The operation has been successfully performed',
                                life: 3000
                            }),
                        100
                    )

                    table.value.selection = null
                    table.value.paging = defaultPaging()
                    await table.value.reload()
                }, 100)
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: view.value.medicamentId
                        ? 'Medicament info update failed'
                        : 'New medicament addition failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.processing = false
        }
    })

    return { table, view, edit }
})

export const useMedicamentSelectorStore = defineStore('medicament-selector', () => {
    const toast = useToast()

    const table = ref({
        dialog: false,
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
        doubleClick() {
            this.select()
        },
        select() {}
    })

    return { table }
})

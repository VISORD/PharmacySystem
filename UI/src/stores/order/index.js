import { defineStore } from 'pinia'
import { ref } from 'vue'
import { FilterMatchMode } from 'primevue/api'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { add, complete, get, history, launch, list, remove, ship } from '@/api/order'

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
            view.value.orderId = this.selection.id
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
                    summary: 'Order has deleted',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order delete failed',
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
        orderId: null,
        dialog: false,
        loading: true,
        processing: true,
        profile: {},
        history: [],
        async reload() {
            this.loading = true

            const response = await get(this.orderId)
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
        },
        async tryDelete() {
            if (!this.orderId) {
                return
            }

            this.processing = false

            const response = await remove(this.orderId)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Order has deleted',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order delete failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.processing = true

            this.dialog = false
            this.orderId = null

            table.value.selection = null
            table.value.paging = defaultPaging()
            await table.value.reload()
        },
        async tryLaunch() {
            if (!this.orderId) {
                return
            }

            this.processing = false

            const response = await launch(this.orderId)
            if (response.status < 400) {
                this.history = response.data.items

                toast.add({
                    severity: 'success',
                    summary: 'Order has launched',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })

                await this.reload()
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order launching failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.processing = true
        },
        async tryShip() {
            if (!this.orderId) {
                return
            }

            this.processing = false

            const response = await ship(this.orderId)
            if (response.status < 400) {
                this.history = response.data.items

                toast.add({
                    severity: 'success',
                    summary: 'Order has shipped',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })

                await this.reload()
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order shipping failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.processing = true
        },
        async tryComplete() {
            if (!this.orderId) {
                return
            }

            this.processing = false

            const response = await complete(this.orderId)
            if (response.status < 400) {
                this.history = response.data.items

                toast.add({
                    severity: 'success',
                    summary: 'Order has completed',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })

                await this.reload()
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order completing failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.processing = true
        },
        async tryGetHistory() {
            if (!this.orderId) {
                return
            }

            const response = await history(this.orderId)
            if (response.status < 400) {
                this.history = response.data.items
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Order history getting failed',
                    detail: response.data.error,
                    life: 3000
                })
            }
        }
    })

    const edit = ref({
        dialog: false,
        pending: false,
        processing: false,
        async tryApply(values) {
            this.processing = true

            const response = await add(values)
            if (response.status < 400) {
                this.dialog = false
                setTimeout(async () => {
                    view.value.orderId = response.data.item.id
                    view.value.dialog = true

                    setTimeout(
                        () =>
                            toast.add({
                                severity: 'success',
                                summary: 'New order added',
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
                    summary: 'New order addition failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.processing = false
        }
    })

    return { table, view, edit }
})

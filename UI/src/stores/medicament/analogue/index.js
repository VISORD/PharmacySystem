import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { analogueList, associate, disassociate, list } from '@/api/medicament'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { useMedicamentStore } from '@/stores/medicament'
import router from '@/plugins/router'

const columns = {
    name: {
        key: 'name',
        field: 'Name',
        header: 'Name',
        matchMode: FilterMatchMode.EQUALS
    },
    type: {
        key: 'type',
        field: 'IsAnalogue',
        header: 'Type',
        matchMode: FilterMatchMode.IN
    },
    vendorPrice: {
        key: 'vendorPrice',
        field: 'VendorPrice',
        header: 'Vendor Price',
        matchMode: FilterMatchMode.EQUALS
    }
}

export const useMedicamentAnalogueStore = defineStore('medicament-analogue', () => {
    const toast = useToast()

    const medicament = useMedicamentStore()

    const table = ref({
        loading: true,
        columns: columns,
        data: {
            items: [],
            totalAmount: 0
        },
        selection: [],
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
            const response = await analogueList(medicament.view.medicamentId, request)

            if (response.status < 400) {
                this.data = response.data
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament analogues load failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.loading = false
        },
        async reset() {
            this.selection = []
            this.filtering = defaultFiltering(columns)
            this.ordering = defaultOrdering()
            this.paging = defaultPaging()
            await this.reload()
        },
        selectRow(selection) {
            this.selection = selection
        },
        selectForContextMenu(selection) {
            this.selection = [selection]
        },
        showInfo() {
            if (this.selection?.length !== 1) {
                return
            }

            window.open(
                router.resolve({
                    path: 'medicament',
                    query: { medicamentId: this.selection[0].id }
                }).href,
                '_blank'
            )
        },
        async tryDisassociate() {
            if (this.selection.length === 0) {
                return
            }

            const analogueIds = this.selection.map((item) => item.id)
            const response = await disassociate(medicament.view.medicamentId, analogueIds)
            if (response.status >= 400 && response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament analogue disassociation failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.paging = defaultPaging()
            await this.reload()
        }
    })

    return { table }
})

export const useMedicamentAnalogueSelectorStore = defineStore('medicament-analogue-selector', () => {
    const toast = useToast()

    const medicament = useMedicamentStore()
    const medicamentAnalogue = useMedicamentAnalogueStore()

    const table = ref({
        dialog: false,
        loading: true,
        columns: { ...medicament.table.columns },
        data: {
            items: [],
            totalAmount: 0
        },
        selection: [],
        filtering: defaultFiltering(medicament.table.columns),
        ordering: defaultOrdering(),
        paging: defaultPaging(),
        async reload({
            filters = undefined,
            orders = undefined,
            pageFirst = undefined,
            pageNumber = undefined,
            pageSize = undefined
        } = {}) {
            if (!medicament.view.medicamentId) {
                return
            }

            this.loading = true

            const request = preparePagingRequest(this, {
                filters,
                extendedFilters: {
                    Id: {
                        matchMode: 'notIn',
                        value: [
                            medicament.view.medicamentId,
                            ...medicamentAnalogue.table.data.items.map((item) => item.id)
                        ]
                    }
                },
                orders,
                pageFirst,
                pageNumber,
                pageSize
            })

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
            this.selection = []
            this.filtering = defaultFiltering(medicament.table.columns)
            this.ordering = defaultOrdering()
            this.paging = defaultPaging()
            await this.reload()
        },
        selectRow(selection) {
            this.selection = selection
        },
        selectForContextMenu(selection) {
            this.selection = [selection]
        },
        showInfo() {
            if (this.selection?.length !== 1) {
                return
            }

            window.open(
                router.resolve({
                    path: 'medicament',
                    query: { medicamentId: this.selection[0].id }
                }).href,
                '_blank'
            )
        },
        async tryAssociate() {
            const analogueIds = this.selection.map((item) => item.id)
            const response = await associate(medicament.view.medicamentId, analogueIds)
            if (response.status >= 400 && response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament analogue association failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            await medicamentAnalogue.table.reset()
            this.dialog = false
        }
    })

    return { table }
})

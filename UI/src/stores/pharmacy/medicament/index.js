import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { list, get } from '@/api/pharmacy/medicament'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { usePharmacyStore } from '@/stores/pharmacy'
import router from '@/plugins/router'

const columns = {
    medicament: {
        key: 'medicament',
        field: 'MedicamentId',
        header: 'Medicament',
        matchMode: FilterMatchMode.EQUALS
    },
    vendorPrice: {
        key: 'vendorPrice',
        field: 'VendorPrice',
        header: 'Vendor Price',
        matchMode: FilterMatchMode.EQUALS
    },
    retailPrice: {
        key: 'retailPrice',
        field: 'RetailPrice',
        header: 'Retail Price',
        matchMode: FilterMatchMode.EQUALS
    },
    quantityOnHand: {
        key: 'quantityOnHand',
        field: 'QuantityOnHand',
        header: 'Quantity on Hand',
        matchMode: FilterMatchMode.EQUALS
    }
}

export const usePharmacyMedicamentStore = defineStore('pharmacy-medicament', () => {
    const toast = useToast()

    const pharmacy = usePharmacyStore()

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
            const response = await list(pharmacy.view.pharmacyId, request)

            if (response.status < 400) {
                this.data = response.data
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy medicaments load failed',
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

            view.value.dialog = true
            view.value.medicamentId = this.selection.medicament.id
        },
        doubleClick() {
            this.showInfo()
        }
    })

    const view = ref({
        medicamentId: null,
        dialog: false,
        loading: true,
        profile: {},
        async reload() {
            this.loading = true

            const response = await get(pharmacy.view.pharmacyId, this.medicamentId)
            if (response.status < 400) {
                this.profile = response.data.item
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy medicament info getting failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.loading = false
        },
        showInfo() {
            if (!this.medicamentId) {
                return
            }

            window.open(
                router.resolve({
                    path: 'medicament',
                    query: { medicamentId: this.medicamentId }
                }).href,
                '_blank'
            )
        },
        doubleClick() {
            this.showInfo()
        }
    })

    return { table, view }
})

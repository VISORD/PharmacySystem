import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { usePharmacyStore } from '@/stores/pharmacy'
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'
import { sale, saleList } from '@/api/pharmacy/medicament'

const columns = {
    soldAt: {
        key: 'soldAt',
        field: 'SoldAt',
        header: 'Sold At',
        matchMode: FilterMatchMode.DATE_IS
    },
    salePrice: {
        key: 'salePrice',
        field: 'SalePrice',
        header: 'Sale Price',
        matchMode: FilterMatchMode.EQUALS
    },
    unitsSold: {
        key: 'unitsSold',
        field: 'UnitsSold',
        header: 'Units Sold',
        matchMode: FilterMatchMode.EQUALS
    }
}

export const usePharmacyMedicamentSaleStore = defineStore('pharmacy-medicament-sale', () => {
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
            const response = await saleList(pharmacy.view.pharmacyId, pharmacyMedicament.view.medicamentId, request)

            if (response.status < 400) {
                this.data = response.data
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy medicament sales load failed',
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
        doubleClick() {}
    })

    const edit = ref({
        dialog: false,
        processing: false,
        data: {},
        async trySave(values) {
            this.processing = true

            const response = await sale(pharmacy.view.pharmacyId, pharmacyMedicament.view.profile.medicament.id, values)
            if (response.status < 400) {
                toast.add({
                    severity: 'success',
                    summary: 'Medicament sale has been added',
                    detail: 'The operation has been successfully performed',
                    life: 3000
                })

                this.dialog = false

                table.value.selection = null
                table.value.paging = defaultPaging()
                await table.value.reload()

                pharmacyMedicament.table.selection = null
                pharmacyMedicament.table.paging = defaultPaging()
                await pharmacyMedicament.table.reload()
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Medicament sale addition failed',
                    detail: response.data.error,
                    life: 3000
                })
            }

            this.processing = false
        }
    })

    return { table, edit }
})

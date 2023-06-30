import { defineStore } from 'pinia'
import { FilterMatchMode } from 'primevue/api'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { preparePagingRequest } from '@/utils/paging'
import { defaultFiltering, defaultOrdering, defaultPaging } from '@/constants/paging'
import { usePharmacyStore } from '@/stores/pharmacy'
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'
import { rateList } from '@/api/pharmacy/medicament'

const columns = {
    retailPrice: {
        key: 'retailPrice',
        field: 'RetailPrice',
        header: 'Retail Price',
        matchMode: FilterMatchMode.EQUALS
    },
    startDate: {
        key: 'startDate',
        field: 'StartDate',
        header: 'Start Date',
        matchMode: FilterMatchMode.DATE_IS
    },
    stopDate: {
        key: 'stopDate',
        field: 'StopDate',
        header: 'Stop Date',
        matchMode: FilterMatchMode.DATE_IS
    }
}

export const usePharmacyMedicamentRateStore = defineStore('pharmacy-medicament-rate', () => {
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
            const response = await rateList(pharmacy.view.pharmacyId, pharmacyMedicament.view.medicamentId, request)

            if (response.status < 400) {
                this.data = response.data
            } else if (response.status !== 401) {
                toast.add({
                    severity: 'error',
                    summary: 'Pharmacy medicament rates load failed',
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
        showInfo() {}
    })

    return { table }
})

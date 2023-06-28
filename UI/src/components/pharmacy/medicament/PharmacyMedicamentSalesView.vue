<script setup>
import ListTable from '@/components/ListTable.vue'
import { usePharmacyMedicamentSaleStore } from '@/stores/pharmacy/medicament/sale'

const pharmacyMedicamentSale = usePharmacyMedicamentSaleStore()
</script>

<template>
    <ListTable :store="pharmacyMedicamentSale">
        <Column
            :key="pharmacyMedicamentSale.table.columns.soldAt.key"
            :field="pharmacyMedicamentSale.table.columns.soldAt.key"
            :header="pharmacyMedicamentSale.table.columns.soldAt.header"
            :sort-field="pharmacyMedicamentSale.table.columns.soldAt.field"
            :filter-field="pharmacyMedicamentSale.table.columns.soldAt.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel }">
                <Calendar
                    input-id="filter-pharmacy-medicament-rates-startDate-input"
                    v-model="filterModel.value"
                    date-format="dd.mm.yy"
                    placeholder="dd.MM.yyyy"
                    mask="99.99.9999"
                />
            </template>

            <template #body="{ data }">
                {{ data.soldAtText }}
            </template>
        </Column>

        <Column
            :key="pharmacyMedicamentSale.table.columns.salePrice.key"
            :field="pharmacyMedicamentSale.table.columns.salePrice.key"
            :header="pharmacyMedicamentSale.table.columns.salePrice.header"
            :sort-field="pharmacyMedicamentSale.table.columns.salePrice.field"
            :filter-field="pharmacyMedicamentSale.table.columns.salePrice.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-sales-salePrice"
                    input-id="filter-pharmacy-medicament-sales-salePrice-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="currency"
                    :max-fraction-digits="4"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                {{ data.salePriceText }}
            </template>
        </Column>

        <Column
            :key="pharmacyMedicamentSale.table.columns.unitsSold.key"
            :field="pharmacyMedicamentSale.table.columns.unitsSold.key"
            :header="pharmacyMedicamentSale.table.columns.unitsSold.header"
            :sortField="pharmacyMedicamentSale.table.columns.unitsSold.field"
            :filterField="pharmacyMedicamentSale.table.columns.unitsSold.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-sales-unitsSold"
                    inputId="filter-pharmacy-medicament-sales-unitsSold-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <template #header>
            <Button type="button" icon="fa-solid fa-calculator" aria-label="Manage sales" />
        </template>
    </ListTable>
</template>

<style scoped></style>

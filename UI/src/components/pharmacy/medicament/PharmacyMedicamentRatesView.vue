<script setup>
import ListTable from '@/components/ListTable.vue'
import { usePharmacyMedicamentRateStore } from '@/stores/pharmacy/medicament/rate'
import { ref } from 'vue'

const pharmacyMedicamentRate = usePharmacyMedicamentRateStore()

const menu = ref([
    {
        label: 'Edit',
        icon: 'fa-solid fa-pencil',
        command: () => {}
    }
])
</script>

<template>
    <ListTable :store="pharmacyMedicamentRate" :menu="menu">
        <Column
            :key="pharmacyMedicamentRate.table.columns.retailPrice.key"
            :field="pharmacyMedicamentRate.table.columns.retailPrice.key"
            :header="pharmacyMedicamentRate.table.columns.retailPrice.header"
            :sort-field="pharmacyMedicamentRate.table.columns.retailPrice.field"
            :filter-field="pharmacyMedicamentRate.table.columns.retailPrice.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-rates-retailPrice"
                    input-id="filter-pharmacy-medicament-rates-retailPrice-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="currency"
                    :max-fraction-digits="4"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                {{ data.retailPriceText }}
            </template>
        </Column>

        <Column
            :key="pharmacyMedicamentRate.table.columns.startDate.key"
            :field="pharmacyMedicamentRate.table.columns.startDate.key"
            :header="pharmacyMedicamentRate.table.columns.startDate.header"
            :sort-field="pharmacyMedicamentRate.table.columns.startDate.field"
            :filter-field="pharmacyMedicamentRate.table.columns.startDate.field"
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
                {{ data.startDateText }}
            </template>
        </Column>

        <Column
            :key="pharmacyMedicamentRate.table.columns.stopDate.key"
            :field="pharmacyMedicamentRate.table.columns.stopDate.key"
            :header="pharmacyMedicamentRate.table.columns.stopDate.header"
            :sort-field="pharmacyMedicamentRate.table.columns.stopDate.field"
            :filter-field="pharmacyMedicamentRate.table.columns.stopDate.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel }">
                <Calendar
                    input-id="filter-pharmacy-medicament-rates-stopDate-input"
                    v-model="filterModel.value"
                    date-format="dd.mm.yy"
                    placeholder="dd.MM.yyyy"
                    mask="99.99.9999"
                />
            </template>

            <template #body="{ data }">
                {{ data.stopDateText }}
            </template>
        </Column>

        <template #header>
            <Button
                type="button"
                icon="fa-solid fa-pencil"
                severity="secondary"
                v-tooltip.left.hover="'Manage rates'"
            />
        </template>
    </ListTable>
</template>

<style scoped></style>

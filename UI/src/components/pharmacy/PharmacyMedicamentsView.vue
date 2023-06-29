<script setup>
import ListTable from '@/components/ListTable.vue'
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'
import { ref } from 'vue'

const pharmacyMedicament = usePharmacyMedicamentStore()

const menu = ref([
    {
        label: 'View',
        icon: 'fa-solid fa-magnifying-glass',
        command: () => pharmacyMedicament.table.showInfo()
    }
])

function buildRateDatesFrameMessage(rate) {
    let message = ''

    if (rate?.startDateText) {
        message += `from ${rate.startDateText}`
    }

    if (rate?.startDateText && rate?.stopDateText) {
        message += ' '
    }

    if (rate?.stopDateText) {
        message += `to ${rate.stopDateText}`
    }

    return message
}
</script>

<template>
    <ListTable :store="pharmacyMedicament" :menu="menu">
        <Column
            :key="pharmacyMedicament.table.columns.medicament.key"
            :field="pharmacyMedicament.table.columns.medicament.key"
            :header="pharmacyMedicament.table.columns.medicament.header"
            :sort-field="pharmacyMedicament.table.columns.medicament.field"
            :filter-field="pharmacyMedicament.table.columns.medicament.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 700"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-pharmacy-medicament-medicament"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="text"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                {{ data.medicament.name }}
            </template>
        </Column>

        <Column
            :key="pharmacyMedicament.table.columns.vendorPrice.key"
            :field="pharmacyMedicament.table.columns.vendorPrice.key"
            :header="pharmacyMedicament.table.columns.vendorPrice.header"
            :sort-field="pharmacyMedicament.table.columns.vendorPrice.field"
            :filter-field="pharmacyMedicament.table.columns.vendorPrice.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-vendorPrice"
                    input-id="filter-pharmacy-medicament-vendorPrice-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="currency"
                    :max-fraction-digits="4"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                {{ data.medicament.vendorPriceText }}
            </template>
        </Column>

        <Column
            :key="pharmacyMedicament.table.columns.retailPrice.key"
            :field="pharmacyMedicament.table.columns.retailPrice.key"
            :header="pharmacyMedicament.table.columns.retailPrice.header"
            :sort-field="pharmacyMedicament.table.columns.retailPrice.field"
            :filter-field="pharmacyMedicament.table.columns.retailPrice.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-retailPrice"
                    input-id="filter-pharmacy-medicament-retailPrice-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="currency"
                    :max-fraction-digits="4"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                <div v-if="data.rate">{{ data.rate?.retailPriceText }}</div>
                <div v-if="data.rate" style="font-size: 10px">({{ buildRateDatesFrameMessage(data.rate) }})</div>
            </template>
        </Column>

        <Column
            :key="pharmacyMedicament.table.columns.quantityOnHand.key"
            :field="pharmacyMedicament.table.columns.quantityOnHand.key"
            :header="pharmacyMedicament.table.columns.quantityOnHand.header"
            :sort-field="pharmacyMedicament.table.columns.quantityOnHand.field"
            :filter-field="pharmacyMedicament.table.columns.quantityOnHand.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-quantityOnHand"
                    input-id="filter-pharmacy-medicament-quantityOnHand-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <template #header>
            <Button type="button" icon="fa-solid fa-plus" aria-label="Create order" />
        </template>
    </ListTable>
</template>

<style scoped></style>

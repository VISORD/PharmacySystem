<script setup>
import ListTable from '@/components/ListTable.vue'
import { useMedicamentAnalogueStore } from '@/stores/medicament/analogue'
import { ref } from 'vue'
import { allMedicamentAnalogueTypes, resolveMedicamentAnalogueType } from '@/constants/medicament-analogue-types'

const medicamentAnalogue = useMedicamentAnalogueStore()

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-arrow-up-right-from-square',
        command: () => medicamentAnalogue.table.showInfo()
    }
])
</script>

<template>
    <ListTable :store="medicamentAnalogue" :menu="menu">
        <Column
            :key="medicamentAnalogue.table.columns.name.key"
            :field="medicamentAnalogue.table.columns.name.key"
            :header="medicamentAnalogue.table.columns.name.header"
            :sort-field="medicamentAnalogue.table.columns.name.field"
            :filter-field="medicamentAnalogue.table.columns.name.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 700"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-medicament-analogue-name"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="text"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="medicamentAnalogue.table.columns.type.key"
            :field="medicamentAnalogue.table.columns.type.key"
            :header="medicamentAnalogue.table.columns.type.header"
            :sortField="medicamentAnalogue.table.columns.type.field"
            :filterField="medicamentAnalogue.table.columns.type.field"
            :sortable="true"
            :showFilterMenu="false"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <MultiSelect
                    id="filter-medicament-analogue-type"
                    inputId="filter-medicament-analogue-type-input"
                    v-model="filterModel.value"
                    :options="allMedicamentAnalogueTypes"
                    optionValue="id"
                    optionLabel="name"
                    class="p-column-filter"
                    @change="filterCallback()"
                >
                    <template #option="{ option }">
                        <span>{{ option.name }}</span>
                    </template>
                </MultiSelect>
            </template>

            <template #body="{ data }">
                {{ resolveMedicamentAnalogueType(data.isAnalogue) }}
            </template>
        </Column>

        <Column
            :key="medicamentAnalogue.table.columns.vendorPrice.key"
            :field="medicamentAnalogue.table.columns.vendorPrice.key"
            :header="medicamentAnalogue.table.columns.vendorPrice.header"
            :sort-field="medicamentAnalogue.table.columns.vendorPrice.field"
            :filter-field="medicamentAnalogue.table.columns.vendorPrice.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-medicament-analogue-vendorPrice"
                    input-id="filter-medicament-analogue-vendorPrice-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="currency"
                    :max-fraction-digits="4"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                {{ data.vendorPriceText }}
            </template>
        </Column>

        <template #header>
            <Button type="button" icon="fa-solid fa-pencil" aria-label="Manage analogues" />
        </template>
    </ListTable>
</template>

<style scoped></style>

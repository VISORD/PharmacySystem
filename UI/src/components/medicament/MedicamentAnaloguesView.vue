<script setup>
import ListTable from '@/components/ListTable.vue'
import { useMedicamentAnaloguesStore } from '@/stores/medicament-analogues'
import { ref } from 'vue'
import { allMedicamentAnalogueTypes, resolveMedicamentAnalogueType } from '@/constants/medicament-analogue-types'

const medicamentAnalogues = useMedicamentAnaloguesStore()

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-magnifying-glass',
        command: () => medicamentAnalogues.table.showInfo()
    }
])
</script>

<template>
    <ListTable :store="medicamentAnalogues" :menu="menu">
        <Column
            :key="medicamentAnalogues.table.columns.name.key"
            :field="medicamentAnalogues.table.columns.name.key"
            :header="medicamentAnalogues.table.columns.name.header"
            :sort-field="medicamentAnalogues.table.columns.name.field"
            :filter-field="medicamentAnalogues.table.columns.name.field"
            :sortable="true"
            filter
            style="min-width: 30rem; max-width: 30rem"
            body-style="font-weight: 600"
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
            :key="medicamentAnalogues.table.columns.type.key"
            :field="medicamentAnalogues.table.columns.type.key"
            :header="medicamentAnalogues.table.columns.type.header"
            :sortField="medicamentAnalogues.table.columns.type.field"
            :filterField="medicamentAnalogues.table.columns.type.field"
            :sortable="true"
            :showFilterMenu="false"
            filter
            style="min-width: 20rem; max-width: 20rem"
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
                {{ resolveMedicamentAnalogueType(data.type) }}
            </template>
        </Column>

        <Column
            :key="medicamentAnalogues.table.columns.vendorPrice.key"
            :field="medicamentAnalogues.table.columns.vendorPrice.key"
            :header="medicamentAnalogues.table.columns.vendorPrice.header"
            :sort-field="medicamentAnalogues.table.columns.vendorPrice.field"
            :filter-field="medicamentAnalogues.table.columns.vendorPrice.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 30rem; max-width: 30rem"
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

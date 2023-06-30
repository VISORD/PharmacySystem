<script setup>
import ListTable from '@/components/ListTable.vue'
import MedicamentAnaloguesSelector from '@/components/medicament/MedicamentAnaloguesSelector.vue'
import { useMedicamentAnalogueSelectorStore, useMedicamentAnalogueStore } from '@/stores/medicament/analogue'
import { ref } from 'vue'
import { allMedicamentAnalogueTypes, resolveMedicamentAnalogueType } from '@/constants/medicament-analogue-types'
import { useConfirm } from 'primevue/useconfirm'

const medicamentAnalogue = useMedicamentAnalogueStore()
const medicamentAnalogueSelector = useMedicamentAnalogueSelectorStore()
const confirm = useConfirm()

function disassociate() {
    confirm.require({
        group: 'medicament-analogue-table-disassociate',
        header: 'Confirmation',
        icon: 'fa-solid fa-triangle-exclamation',
        acceptIcon: 'fa-solid fa-check',
        rejectIcon: 'fa-solid fa-xmark',
        accept: async () => await medicamentAnalogue.table.tryDisassociate(),
        reject: () => {}
    })
}

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-arrow-up-right-from-square',
        command: () => medicamentAnalogue.table.showInfo()
    },
    {
        label: 'Disassociate',
        icon: 'fa-solid fa-link-slash',
        command: () => disassociate()
    }
])
</script>

<template>
    <ConfirmDialog group="medicament-analogue-table-disassociate">
        <template #message>
            <div>
                Are you sure you want to disassociate selected medicament{{
                    medicamentAnalogue.table.selection.length > 1 ? 's' : ''
                }}?
            </div>
        </template>
    </ConfirmDialog>

    <MedicamentAnaloguesSelector />

    <ListTable :store="medicamentAnalogue" :menu="menu" selection-mode="multiple">
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
            <div>
                <Button
                    type="button"
                    severity="secondary"
                    icon="fa-solid fa-link"
                    style="margin-right: 1rem"
                    @click="medicamentAnalogueSelector.table.dialog = true"
                    v-tooltip.left.hover="'Associate analogues'"
                />

                <Button
                    type="button"
                    severity="danger"
                    icon="fa-solid fa-link-slash"
                    aria-label="Manage analogues"
                    :disabled="medicamentAnalogue.table.selection.length === 0"
                    @click="disassociate()"
                    v-tooltip.left.hover="'Disassociate analogues'"
                    style="transition: all; transition-duration: 0.25s"
                />
            </div>
        </template>
    </ListTable>
</template>

<style scoped></style>

<script setup>
import ListTable from '@/components/ListTable.vue'
import MedicamentProfileView from '@/components/medicament/MedicamentProfileView.vue'
import { useMedicamentStore } from '@/stores/medicament'
import { ref } from 'vue'
import { useConfirm } from 'primevue/useconfirm'

const medicament = useMedicamentStore()
const confirm = useConfirm()

const menu = ref([
    {
        label: 'View',
        icon: 'fa-solid fa-magnifying-glass',
        command: async () => await medicament.table.showInfo()
    },
    {
        label: 'Delete',
        icon: 'fa-solid fa-trash-can',
        command: () => {
            confirm.require({
                group: 'medicament-delete',
                header: 'Confirmation',
                icon: 'fa-solid fa-triangle-exclamation',
                acceptIcon: 'fa-solid fa-check',
                rejectIcon: 'fa-solid fa-xmark',
                accept: async () => await medicament.table.tryDelete(),
                reject: () => {}
            })
        }
    }
])
</script>

<template>
    <ConfirmDialog group="medicament-delete">
        <template #message>
            <div>
                Are you sure you want to delete '<b>{{ medicament.table.selection.name }}</b
                >' medicament?
            </div>
        </template>
    </ConfirmDialog>

    <MedicamentProfileView />

    <ListTable :store="medicament" :menu="menu">
        <Column
            :key="medicament.table.columns.name.key"
            :field="medicament.table.columns.name.key"
            :header="medicament.table.columns.name.header"
            :sort-field="medicament.table.columns.name.field"
            :filter-field="medicament.table.columns.name.field"
            :sortable="true"
            filter
            style="min-width: 30rem; max-width: 30rem"
            body-style="font-weight: 600"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-medicament-name"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="text"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="medicament.table.columns.vendorPrice.key"
            :field="medicament.table.columns.vendorPrice.key"
            :header="medicament.table.columns.vendorPrice.header"
            :sort-field="medicament.table.columns.vendorPrice.field"
            :filter-field="medicament.table.columns.vendorPrice.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 30rem; max-width: 30rem"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-medicament-vendorPrice"
                    input-id="filter-medicament-vendorPrice-input"
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
            <Button type="button" icon="fa-solid fa-plus" aria-label="Add new medicament" />
        </template>
    </ListTable>
</template>

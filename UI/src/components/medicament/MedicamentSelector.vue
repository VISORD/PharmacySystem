<script setup>
import ListTable from '@/components/ListTable.vue'
import { ref } from 'vue'
import router from '@/plugins/router'
import { useMedicamentSelectorStore } from '@/stores/medicament'

const emits = defineEmits(['apply'])

const medicamentSelector = useMedicamentSelectorStore()
medicamentSelector.table.select = () => {
    if (!medicamentSelector.table.selection) {
        return
    }

    emits('apply', {
        medicament: medicamentSelector.table.selection
    })

    medicamentSelector.table.dialog = false
}

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-arrow-up-right-from-square',
        command: () => {
            if (!medicamentSelector.table.selection) {
                return
            }

            window.open(
                router.resolve({
                    path: 'medicament',
                    query: { medicamentId: medicamentSelector.table.selection.id }
                }).href,
                '_blank'
            )
        }
    }
])
</script>

<template>
    <Dialog
        v-model:visible="medicamentSelector.table.dialog"
        modal
        position="top"
        :draggable="false"
        dismissable-mask
        header="Choose the medicament"
        style="width: 70rem; margin: 5rem"
    >
        <ListTable :store="medicamentSelector" :menu="menu">
            <Column
                :key="medicamentSelector.table.columns.name.key"
                :field="medicamentSelector.table.columns.name.key"
                :header="medicamentSelector.table.columns.name.header"
                :sort-field="medicamentSelector.table.columns.name.field"
                :filter-field="medicamentSelector.table.columns.name.field"
                :sortable="true"
                filter
                style="min-width: 30rem; max-width: 30rem"
                body-style="font-weight: 700"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputText
                        id="filter-medicament-selector-name"
                        v-model="filterModel.value"
                        v-tooltip.top.focus="'Hit enter key to filter'"
                        type="text"
                        @keydown.enter="filterCallback()"
                        class="p-column-filter"
                    />
                </template>
            </Column>

            <Column
                :key="medicamentSelector.table.columns.vendorPrice.key"
                :field="medicamentSelector.table.columns.vendorPrice.key"
                :header="medicamentSelector.table.columns.vendorPrice.header"
                :sort-field="medicamentSelector.table.columns.vendorPrice.field"
                :filter-field="medicamentSelector.table.columns.vendorPrice.field"
                :sortable="true"
                dataType="numeric"
                filter
                style="min-width: 30rem; max-width: 30rem"
                body-style="font-weight: 500"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputNumber
                        id="filter-medicament-selector-vendorPrice"
                        input-id="filter-medicament-selector-vendorPrice-input"
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
        </ListTable>

        <small class="p-error" id="text-error">
            {{ !medicamentSelector.table.selection ? 'Select any medicament' : '&nbsp;' }}
        </small>

        <div style="text-align: right">
            <div class="buttons">
                <Button label="Cancel" icon="fa-solid fa-xmark" @click="medicamentSelector.table.dialog = false" text />
                <Button label="Apply" icon="fa-solid fa-check" @click="medicamentSelector.table.select()" />
            </div>
        </div>
    </Dialog>
</template>

<script setup>
import ListTable from '@/components/ListTable.vue'
import { ref } from 'vue'
import { useConfirm } from 'primevue/useconfirm'
import { useMedicamentAnalogueSelectorStore } from '@/stores/medicament/analogue'
import { useMedicamentStore } from '@/stores/medicament'

const medicament = useMedicamentStore()
const medicamentAnalogueSelector = useMedicamentAnalogueSelectorStore()
const confirm = useConfirm()

function associate() {
    if (!medicamentAnalogueSelector.table.selection.length) {
        return
    }

    confirm.require({
        group: 'medicament-analogue-table-associate',
        header: 'Confirmation',
        icon: 'fa-solid fa-triangle-exclamation',
        acceptIcon: 'fa-solid fa-check',
        rejectIcon: 'fa-solid fa-xmark',
        accept: async () => await medicamentAnalogueSelector.table.tryAssociate(),
        reject: () => {}
    })
}

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-arrow-up-right-from-square',
        command: () => medicamentAnalogueSelector.table.showInfo()
    },
    {
        label: 'Associate',
        icon: 'fa-solid fa-link',
        command: () => associate()
    }
])
</script>

<template>
    <ConfirmDialog group="medicament-analogue-table-associate">
        <template #message>
            <div>
                Are you sure you want to associate selected medicament{{
                    medicamentAnalogueSelector.table.selection.length > 1 ? 's' : ''
                }}?
            </div>
        </template>
    </ConfirmDialog>

    <Dialog
        v-model:visible="medicamentAnalogueSelector.table.dialog"
        modal
        position="top"
        :draggable="false"
        dismissable-mask
        :header="
            medicament.view.medicamentId
                ? `Choose medicament analogues for ${medicament.view.profile.name}`
                : 'Choose medicament analogues'
        "
        style="width: 70rem; margin: 5rem"
    >
        <ListTable :store="medicamentAnalogueSelector" :menu="menu" selection-mode="multiple">
            <Column
                :key="medicamentAnalogueSelector.table.columns.name.key"
                :field="medicamentAnalogueSelector.table.columns.name.key"
                :header="medicamentAnalogueSelector.table.columns.name.header"
                :sort-field="medicamentAnalogueSelector.table.columns.name.field"
                :filter-field="medicamentAnalogueSelector.table.columns.name.field"
                :sortable="true"
                filter
                style="min-width: 30rem; max-width: 30rem"
                body-style="font-weight: 700"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputText
                        id="filter-medicament-analogue-selector-name"
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
                body-style="font-weight: 500"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputNumber
                        id="filter-medicament-analogue-selector-vendorPrice"
                        input-id="filter-medicament-analogue-selector-vendorPrice-input"
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
            {{ !medicamentAnalogueSelector.table.selection.length ? 'Select any medicament' : '&nbsp;' }}
        </small>

        <div style="text-align: right">
            <div class="buttons">
                <Button
                    label="Cancel"
                    icon="fa-solid fa-xmark"
                    @click="medicamentAnalogueSelector.table.dialog = false"
                    text
                />

                <Button label="Apply" icon="fa-solid fa-check" @click="associate()" />
            </div>
        </div>
    </Dialog>
</template>

<script setup>
import ListTable from '@/components/ListTable.vue'
import { ref } from 'vue'
import { usePharmacySelectorStore } from '@/stores/pharmacy'
import router from '@/plugins/router'

const emits = defineEmits(['apply'])

const pharmacySelector = usePharmacySelectorStore()
pharmacySelector.table.select = () => {
    if (!pharmacySelector.table.selection) {
        return
    }

    emits('apply', {
        pharmacy: pharmacySelector.table.selection
    })

    pharmacySelector.table.dialog = false
}

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-arrow-up-right-from-square',
        command: () => {
            if (!pharmacySelector.table.selection) {
                return
            }

            window.open(
                router.resolve({
                    path: 'pharmacy',
                    query: { pharmacyId: pharmacySelector.table.selection.id }
                }).href,
                '_blank'
            )
        }
    }
])
</script>

<template>
    <Dialog
        v-model:visible="pharmacySelector.table.dialog"
        modal
        position="top"
        :draggable="false"
        dismissable-mask
        header="Choose the pharmacy"
        style="width: 70rem; margin: 5rem"
    >
        <ListTable :store="pharmacySelector" :menu="menu">
            <Column
                :key="pharmacySelector.table.columns.name.key"
                :field="pharmacySelector.table.columns.name.key"
                :header="pharmacySelector.table.columns.name.header"
                :sort-field="pharmacySelector.table.columns.name.field"
                :filter-field="pharmacySelector.table.columns.name.field"
                :sortable="true"
                filter
                style="min-width: 20rem; max-width: 20rem"
                body-style="font-weight: 700"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputText
                        id="filter-pharmacy-selector-name"
                        v-model="filterModel.value"
                        v-tooltip.top.focus="'Hit enter key to filter'"
                        type="text"
                        @keydown.enter="filterCallback()"
                        class="p-column-filter"
                    />
                </template>
            </Column>

            <Column
                :key="pharmacySelector.table.columns.email.key"
                :field="pharmacySelector.table.columns.email.key"
                :header="pharmacySelector.table.columns.email.header"
                :sort-field="pharmacySelector.table.columns.email.field"
                :filter-field="pharmacySelector.table.columns.email.field"
                :sortable="true"
                filter
                style="min-width: 20rem; max-width: 20rem"
                body-style="font-weight: 500"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputText
                        id="filter-pharmacy-selector-email"
                        v-model="filterModel.value"
                        v-tooltip.top.focus="'Hit enter key to filter'"
                        type="text"
                        @keydown.enter="filterCallback()"
                        class="p-column-filter"
                    />
                </template>

                <template #body="{ data }">
                    {{ data.email ?? '—' }}
                </template>
            </Column>

            <Column
                :key="pharmacySelector.table.columns.phone.key"
                :field="pharmacySelector.table.columns.phone.key"
                :header="pharmacySelector.table.columns.phone.header"
                :sort-field="pharmacySelector.table.columns.phone.field"
                :filter-field="pharmacySelector.table.columns.phone.field"
                :sortable="true"
                filter
                style="min-width: 20rem; max-width: 20rem"
                body-style="font-weight: 500"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputText
                        id="filter-pharmacy-selector-phone"
                        v-model="filterModel.value"
                        v-tooltip.top.focus="'Hit enter key to filter'"
                        type="text"
                        @keydown.enter="filterCallback()"
                        class="p-column-filter"
                    />
                </template>

                <template #body="{ data }">
                    {{ data.phone ?? '—' }}
                </template>
            </Column>

            <Column
                :key="pharmacySelector.table.columns.address.key"
                :field="pharmacySelector.table.columns.address.key"
                :header="pharmacySelector.table.columns.address.header"
                :sort-field="pharmacySelector.table.columns.address.field"
                :filter-field="pharmacySelector.table.columns.address.field"
                :sortable="true"
                filter
                style="min-width: 40rem; max-width: 40rem"
                body-style="font-weight: 500"
            >
                <template #filter="{ filterModel, filterCallback }">
                    <InputText
                        id="filter-pharmacy-selector-address"
                        v-model="filterModel.value"
                        v-tooltip.top.focus="'Hit enter key to filter'"
                        type="text"
                        @keydown.enter="filterCallback()"
                        class="p-column-filter"
                    />
                </template>
            </Column>
        </ListTable>

        <small class="p-error" id="text-error">
            {{ !pharmacySelector.table.selection ? 'Select any pharmacy' : '&nbsp;' }}
        </small>

        <div style="text-align: right">
            <div class="buttons">
                <Button label="Cancel" icon="fa-solid fa-xmark" @click="pharmacySelector.table.dialog = false" text />
                <Button label="Apply" icon="fa-solid fa-check" @click="pharmacySelector.table.select()" />
            </div>
        </div>
    </Dialog>
</template>

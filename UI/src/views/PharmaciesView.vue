<script setup>
import ListTable from '@/components/ListTable.vue'
import PharmacyProfileView from '@/components/pharmacy/PharmacyProfileView.vue'
import PharmacyMedicamentProfileView from '@/components/pharmacy/medicament/PharmacyMedicamentProfileView.vue'
import PharmacyInfoForm from '@/components/pharmacy/PharmacyInfoForm.vue'
import PharmacyMedicamentRateForm from '@/components/pharmacy/medicament/PharmacyMedicamentRateForm.vue'
import PharmacyMedicamentSaleForm from '@/components/pharmacy/medicament/PharmacyMedicamentSaleForm.vue'
import { usePharmacyStore } from '@/stores/pharmacy'
import { ref } from 'vue'
import { useConfirm } from 'primevue/useconfirm'

const pharmacy = usePharmacyStore()
const confirm = useConfirm()

const menu = ref([
    {
        label: 'View',
        icon: 'fa-solid fa-magnifying-glass',
        command: async () => await pharmacy.table.showInfo()
    },
    {
        label: 'Edit',
        icon: 'fa-solid fa-pencil',
        command: async () => {
            await pharmacy.table.showInfo()
            pharmacy.edit.pending = true
        }
    },
    {
        label: 'Delete',
        icon: 'fa-solid fa-trash-can',
        command: () => {
            confirm.require({
                group: 'pharmacy-table-delete',
                header: 'Confirmation',
                icon: 'fa-solid fa-triangle-exclamation',
                acceptIcon: 'fa-solid fa-check',
                rejectIcon: 'fa-solid fa-xmark',
                accept: async () => await pharmacy.table.tryDelete(),
                reject: () => {}
            })
        }
    }
])
</script>

<template>
    <ConfirmDialog group="pharmacy-table-delete">
        <template #message>
            <div>
                Are you sure you want to delete '<b>{{ pharmacy.table.selection.name }}</b
                >' pharmacy?
            </div>
        </template>
    </ConfirmDialog>

    <PharmacyProfileView />
    <PharmacyMedicamentProfileView />
    <PharmacyInfoForm />
    <PharmacyMedicamentRateForm />
    <PharmacyMedicamentSaleForm />

    <ListTable :store="pharmacy" :menu="menu">
        <Column
            :key="pharmacy.table.columns.name.key"
            :field="pharmacy.table.columns.name.key"
            :header="pharmacy.table.columns.name.header"
            :sort-field="pharmacy.table.columns.name.field"
            :filter-field="pharmacy.table.columns.name.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 700"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-pharmacy-name"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="text"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="pharmacy.table.columns.email.key"
            :field="pharmacy.table.columns.email.key"
            :header="pharmacy.table.columns.email.header"
            :sort-field="pharmacy.table.columns.email.field"
            :filter-field="pharmacy.table.columns.email.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-pharmacy-email"
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
            :key="pharmacy.table.columns.phone.key"
            :field="pharmacy.table.columns.phone.key"
            :header="pharmacy.table.columns.phone.header"
            :sort-field="pharmacy.table.columns.phone.field"
            :filter-field="pharmacy.table.columns.phone.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-pharmacy-phone"
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
            :key="pharmacy.table.columns.address.key"
            :field="pharmacy.table.columns.address.key"
            :header="pharmacy.table.columns.address.header"
            :sort-field="pharmacy.table.columns.address.field"
            :filter-field="pharmacy.table.columns.address.field"
            :sortable="true"
            filter
            style="min-width: 40rem; max-width: 40rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-pharmacy-address"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="text"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <template #header>
            <Button
                type="button"
                icon="fa-solid fa-plus"
                severity="secondary"
                v-tooltip.left.hover="'Add new pharmacy'"
                @click="pharmacy.edit.dialog = true"
                :disabled="pharmacy.table.loading"
            />
        </template>
    </ListTable>
</template>

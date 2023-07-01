<script setup>
import ListTable from '@/components/ListTable.vue'
import OrderMedicamentInfoFrom from '@/components/order/OrderMedicamentInfoFrom.vue'
import { useOrderStore } from '@/stores/order'
import { useOrderMedicamentStore } from '@/stores/order/medicament'
import { ref } from 'vue'
import { allYesNoOptions, resolveYesNoOption } from '@/constants/yes-no-options'
import { DRAFT, ORDERED } from '@/constants/order-statuses'
import { useConfirm } from 'primevue/useconfirm'

const order = useOrderStore()
const orderMedicament = useOrderMedicamentStore()
orderMedicament.table.doubleClick = () => {
    orderMedicament.edit.orderMedicament = orderMedicament.table.selection
    orderMedicament.edit.dialog = true
}

const confirm = useConfirm()

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-arrow-up-right-from-square',
        command: () => orderMedicament.table.showInfo()
    },
    {
        label: 'Request',
        disabled: () => order.view.profile.status !== DRAFT.id,
        items: [
            {
                label: 'Change count',
                icon: 'fa-solid fa-calculator',
                command: () => {
                    orderMedicament.edit.orderMedicament = orderMedicament.table.selection
                    orderMedicament.edit.dialog = true
                }
            },
            {
                label: 'Delete',
                icon: 'fa-solid fa-times',
                command: () => {}
            }
        ]
    },
    {
        label: 'Approval',
        disabled: () => order.view.profile.status !== ORDERED.id,
        items: [
            {
                label: () => (orderMedicament.table.selection?.isApproved !== true ? 'Approve' : 'Re-approve'),
                icon: 'fa-solid fa-check',
                command: () => {
                    orderMedicament.edit.orderMedicament = orderMedicament.table.selection
                    orderMedicament.edit.dialog = true
                }
            },
            {
                label: 'Disapprove',
                icon: 'fa-solid fa-times',
                command: () => {
                    confirm.require({
                        group: 'order-medicaments-table-disapprove',
                        header: 'Confirmation',
                        icon: 'fa-solid fa-triangle-exclamation',
                        acceptIcon: 'fa-solid fa-check',
                        rejectIcon: 'fa-solid fa-xmark',
                        accept: async () => await orderMedicament.table.tryDisapprove(),
                        reject: () => {}
                    })
                },
                disabled: () => orderMedicament.table.selection?.isApproved !== true
            }
        ]
    }
])
</script>

<template>
    <ConfirmDialog group="order-medicaments-table-disapprove">
        <template #message>
            <div>
                Are you sure you want to disapprove '<b>{{ orderMedicament.table.selection.medicament.name }}</b
                >' medicament?
            </div>
        </template>
    </ConfirmDialog>

    <OrderMedicamentInfoFrom />

    <ListTable :store="orderMedicament" :menu="menu">
        <Column
            :key="orderMedicament.table.columns.medicament.key"
            :field="orderMedicament.table.columns.medicament.key"
            :header="orderMedicament.table.columns.medicament.header"
            :sort-field="orderMedicament.table.columns.medicament.field"
            :filter-field="orderMedicament.table.columns.medicament.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 700"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-order-medicament-medicament"
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
            :key="orderMedicament.table.columns.quantityOnHand.key"
            :field="orderMedicament.table.columns.quantityOnHand.key"
            :header="orderMedicament.table.columns.quantityOnHand.header"
            :sort-field="orderMedicament.table.columns.quantityOnHand.field"
            :filter-field="orderMedicament.table.columns.quantityOnHand.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-order-medicament-quantityOnHand"
                    input-id="filter-order-medicament-quantityOnHand-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="orderMedicament.table.columns.requestedCount.key"
            :field="orderMedicament.table.columns.requestedCount.key"
            :header="orderMedicament.table.columns.requestedCount.header"
            :sort-field="orderMedicament.table.columns.requestedCount.field"
            :filter-field="orderMedicament.table.columns.requestedCount.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-order-medicament-requestedCount"
                    input-id="filter-order-medicament-requestedCount-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="orderMedicament.table.columns.approvedCount.key"
            :field="orderMedicament.table.columns.approvedCount.key"
            :header="orderMedicament.table.columns.approvedCount.header"
            :sort-field="orderMedicament.table.columns.approvedCount.field"
            :filter-field="orderMedicament.table.columns.approvedCount.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-order-medicament-approvedCount"
                    input-id="filter-order-medicament-approvedCount-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                <div v-if="data.approvedCount">{{ data.approvedCount }}</div>
                <div v-else>—</div>
            </template>
        </Column>

        <Column
            :key="orderMedicament.table.columns.isApproved.key"
            :field="orderMedicament.table.columns.isApproved.key"
            :header="orderMedicament.table.columns.isApproved.header"
            :sortField="orderMedicament.table.columns.isApproved.field"
            :filterField="orderMedicament.table.columns.isApproved.field"
            :sortable="true"
            :showFilterMenu="false"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <MultiSelect
                    id="filter-order-status"
                    inputId="filter-order-status-input"
                    v-model="filterModel.value"
                    :options="allYesNoOptions"
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
                {{ resolveYesNoOption(data.isApproved) }}
            </template>
        </Column>

        <template #header>
            <Button
                v-if="order.view.profile.status === DRAFT.id"
                type="button"
                icon="fa-solid fa-plus"
                severity="secondary"
                v-tooltip.left.hover="'Request a medicament'"
                @click="orderMedicament.edit.dialog = true"
            />
        </template>
    </ListTable>
</template>

<style scoped></style>

<script setup>
import ListTable from '@/components/ListTable.vue'
import OrderProfileView from '@/components/order/OrderProfileView.vue'
import OrderInfoForm from '@/components/order/OrderInfoForm.vue'
import { allOrderStatuses, resolveOrderStatus } from '@/constants/order-statuses'
import { useOrderStore } from '@/stores/order'
import { ref } from 'vue'
import { useConfirm } from 'primevue/useconfirm'

const order = useOrderStore()
const confirm = useConfirm()

const menu = ref([
    {
        label: 'View',
        icon: 'fa-solid fa-magnifying-glass',
        command: async () => await order.table.showInfo()
    },
    {
        label: 'Delete',
        icon: 'fa-solid fa-trash-can',
        command: () => {
            confirm.require({
                group: 'order-table-delete',
                header: 'Confirmation',
                icon: 'fa-solid fa-triangle-exclamation',
                acceptIcon: 'fa-solid fa-check',
                rejectIcon: 'fa-solid fa-xmark',
                accept: async () => await order.table.tryDelete(),
                reject: () => {}
            })
        }
    }
])
</script>

<template>
    <ConfirmDialog group="order-table-delete">
        <template #message>
            <div>
                Are you sure you want to delete order
                <b>#{{ order.table.selection.id }}</b
                >?
            </div>
        </template>
    </ConfirmDialog>

    <OrderProfileView />
    <OrderInfoForm />

    <ListTable :store="order" :menu="menu">
        <Column
            :key="order.table.columns.id.key"
            :field="order.table.columns.id.key"
            :header="order.table.columns.id.header"
            :sortField="order.table.columns.id.field"
            :filterField="order.table.columns.id.field"
            :sortable="true"
            filter
            style="min-width: 15rem; max-width: 15rem"
            body-style="text-align: center; font-size: 16px; font-weight: 700"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-order-id"
                    inputId="filter-order-id-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="order.table.columns.pharmacy.key"
            :field="order.table.columns.pharmacy.key"
            :header="order.table.columns.pharmacy.header"
            :sortField="order.table.columns.pharmacy.field"
            :filterField="order.table.columns.pharmacy.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputText
                    id="filter-order-pharmacy"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    type="text"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>

            <template #body="{ data }">
                <div>{{ data.pharmacy.name }}</div>
                <div style="font-size: 10px">{{ data.pharmacy.address }}</div>
            </template>
        </Column>

        <Column
            :key="order.table.columns.medicamentItemCount.key"
            :field="order.table.columns.medicamentItemCount.key"
            :header="order.table.columns.medicamentItemCount.header"
            :sortField="order.table.columns.medicamentItemCount.field"
            :filterField="order.table.columns.medicamentItemCount.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-order-medicamentItemCount"
                    inputId="filter-order-medicamentItemCount-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="order.table.columns.status.key"
            :field="order.table.columns.status.key"
            :header="order.table.columns.status.header"
            :sortField="order.table.columns.status.field"
            :filterField="order.table.columns.status.field"
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
                    :options="allOrderStatuses"
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
                {{ resolveOrderStatus(data.status) }}
            </template>
        </Column>

        <Column
            :key="order.table.columns.orderedAt.key"
            :field="order.table.columns.orderedAt.key"
            :header="order.table.columns.orderedAt.header"
            :sort-field="order.table.columns.orderedAt.field"
            :filter-field="order.table.columns.orderedAt.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel }">
                <Calendar
                    input-id="filter-order-orderedAt-input"
                    v-model="filterModel.value"
                    date-format="dd.mm.yy"
                    placeholder="dd.MM.yyyy"
                    mask="99.99.9999"
                />
            </template>

            <template #body="{ data }">
                {{ data.orderedAtText ?? '—' }}
            </template>
        </Column>

        <Column
            :key="order.table.columns.updatedAt.key"
            :field="order.table.columns.updatedAt.key"
            :header="order.table.columns.updatedAt.header"
            :sort-field="order.table.columns.updatedAt.field"
            :filter-field="order.table.columns.updatedAt.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel }">
                <Calendar
                    input-id="filter-order-updatedAt-input"
                    v-model="filterModel.value"
                    date-format="dd.mm.yy"
                    placeholder="dd.MM.yyyy"
                    mask="99.99.9999"
                />
            </template>

            <template #body="{ data }">
                {{ data.updatedAtText }}
            </template>
        </Column>

        <template #header>
            <Button
                type="button"
                icon="fa-solid fa-plus"
                severity="secondary"
                v-tooltip.left.hover="'Add new order'"
                @click="order.edit.dialog = true"
            />
        </template>
    </ListTable>
</template>

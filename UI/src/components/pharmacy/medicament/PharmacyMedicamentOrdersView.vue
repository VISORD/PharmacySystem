<script setup>
import ListTable from '@/components/ListTable.vue'
import { usePharmacyMedicamentOrderStore } from '@/stores/pharmacy/medicament/order'
import { ref } from 'vue'
import { allOrderStatuses, resolveOrderStatus } from '@/constants/order-statuses'

const pharmacyMedicamentOrder = usePharmacyMedicamentOrderStore()

const menu = ref([
    {
        label: 'View in new window',
        icon: 'fa-solid fa-arrow-up-right-from-square',
        command: () => pharmacyMedicamentOrder.table.showInfo()
    }
])
</script>

<template>
    <ListTable :store="pharmacyMedicamentOrder" :menu="menu">
        <Column
            :key="pharmacyMedicamentOrder.table.columns.orderId.key"
            :field="pharmacyMedicamentOrder.table.columns.orderId.key"
            :header="pharmacyMedicamentOrder.table.columns.orderId.header"
            :sortField="pharmacyMedicamentOrder.table.columns.orderId.field"
            :filterField="pharmacyMedicamentOrder.table.columns.orderId.field"
            :sortable="true"
            filter
            style="min-width: 15rem; max-width: 15rem"
            body-style="text-align: center; font-size: 16px; font-weight: 700"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-orders-id"
                    inputId="filter-pharmacy-medicament-orders-id-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="pharmacyMedicamentOrder.table.columns.orderCount.key"
            :field="pharmacyMedicamentOrder.table.columns.orderCount.key"
            :header="pharmacyMedicamentOrder.table.columns.orderCount.header"
            :sortField="pharmacyMedicamentOrder.table.columns.orderCount.field"
            :filterField="pharmacyMedicamentOrder.table.columns.orderCount.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <InputNumber
                    id="filter-pharmacy-medicament-orders-orderCount"
                    inputId="filter-pharmacy-medicament-orders-orderCount-input"
                    v-model="filterModel.value"
                    v-tooltip.top.focus="'Hit enter key to filter'"
                    @keydown.enter="filterCallback()"
                    class="p-column-filter"
                />
            </template>
        </Column>

        <Column
            :key="pharmacyMedicamentOrder.table.columns.status.key"
            :field="pharmacyMedicamentOrder.table.columns.status.key"
            :header="pharmacyMedicamentOrder.table.columns.status.header"
            :sortField="pharmacyMedicamentOrder.table.columns.status.field"
            :filterField="pharmacyMedicamentOrder.table.columns.status.field"
            :sortable="true"
            :showFilterMenu="false"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel, filterCallback }">
                <MultiSelect
                    id="filter-order-status"
                    inputId="filter-pharmacy-medicament-orders-status-input"
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
            :key="pharmacyMedicamentOrder.table.columns.orderedAt.key"
            :field="pharmacyMedicamentOrder.table.columns.orderedAt.key"
            :header="pharmacyMedicamentOrder.table.columns.orderedAt.header"
            :sort-field="pharmacyMedicamentOrder.table.columns.orderedAt.field"
            :filter-field="pharmacyMedicamentOrder.table.columns.orderedAt.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel }">
                <Calendar
                    input-id="filter-pharmacy-medicament-orders-orderedAt-input"
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
            :key="pharmacyMedicamentOrder.table.columns.updatedAt.key"
            :field="pharmacyMedicamentOrder.table.columns.updatedAt.key"
            :header="pharmacyMedicamentOrder.table.columns.updatedAt.header"
            :sort-field="pharmacyMedicamentOrder.table.columns.updatedAt.field"
            :filter-field="pharmacyMedicamentOrder.table.columns.updatedAt.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 500"
        >
            <template #filter="{ filterModel }">
                <Calendar
                    input-id="filter-pharmacy-medicament-orders-updatedAt-input"
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
                v-tooltip.left.hover="'Add an order'"
                :disabled="pharmacyMedicamentOrder.table.loading"
            />
        </template>
    </ListTable>
</template>

<style scoped></style>

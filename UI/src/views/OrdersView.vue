<script setup>
import { ref } from 'vue'
import { list } from '@/api/order'
import { FilterMatchMode } from 'primevue/api'
import { allOrderStatuses, resolveOrderStatus } from '@/constants/order-statuses'
import ListTable from '@/components/ListTable.vue'

const pharmacy = ref({
    key: 'pharmacy',
    field: 'Pharmacy',
    header: 'Pharmacy',
    matchMode: FilterMatchMode.EQUALS
})

const medicamentItemCount = ref({
    key: 'medicamentItemCount',
    field: 'MedicamentItemCount',
    header: 'Medicament Item Count',
    matchMode: FilterMatchMode.EQUALS
})

const status = ref({
    key: 'status',
    field: 'Status',
    header: 'Status',
    matchMode: FilterMatchMode.IN
})

const orderedAt = ref({
    key: 'orderedAt',
    field: 'OrderedAt',
    header: 'Ordered At',
    matchMode: FilterMatchMode.DATE_IS
})

const updatedAt = ref({
    key: 'updatedAt',
    field: 'UpdatedAt',
    header: 'Updated At',
    matchMode: FilterMatchMode.DATE_IS
})
</script>

<template>
    <ListTable :columns="[pharmacy, medicamentItemCount, status, orderedAt, updatedAt]" :list="list">
        <Column
            :key="pharmacy.key"
            :field="pharmacy.key"
            :header="pharmacy.header"
            :sortField="pharmacy.field"
            :filterField="pharmacy.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
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
                <div style="font-weight: 600">{{ data.pharmacy.name }}</div>
                <div style="font-size: 10px">{{ data.pharmacy.address }}</div>
            </template>
        </Column>

        <Column
            :key="medicamentItemCount.key"
            :field="medicamentItemCount.key"
            :header="medicamentItemCount.header"
            :sortField="medicamentItemCount.field"
            :filterField="medicamentItemCount.field"
            :sortable="true"
            dataType="numeric"
            filter
            style="min-width: 20rem; max-width: 20rem"
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
            :key="status.key"
            :field="status.key"
            :header="status.header"
            :sortField="status.field"
            :filterField="status.field"
            :sortable="true"
            :showFilterMenu="false"
            filter
            style="min-width: 20rem; max-width: 20rem"
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
            :key="orderedAt.key"
            :field="orderedAt.key"
            :header="orderedAt.header"
            :sort-field="orderedAt.field"
            :filter-field="orderedAt.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
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
                {{ data.orderedAtText }}
            </template>
        </Column>

        <Column
            :key="updatedAt.key"
            :field="updatedAt.key"
            :header="updatedAt.header"
            :sort-field="updatedAt.field"
            :filter-field="updatedAt.field"
            :sortable="true"
            data-type="date"
            filter
            style="min-width: 20rem; max-width: 20rem"
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
            <Button type="button" icon="fa-solid fa-plus" aria-label="Add new order" />
        </template>
    </ListTable>
</template>

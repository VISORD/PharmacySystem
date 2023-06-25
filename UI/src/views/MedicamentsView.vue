<script setup>
import { ref } from 'vue'
import ListTable from '@/components/ListTable.vue'
import { FilterMatchMode } from 'primevue/api'
import { list } from '@/api/medicament'

const name = ref({
    key: 'name',
    field: 'Name',
    header: 'Name',
    matchMode: FilterMatchMode.EQUALS
})

const vendorPrice = ref({
    key: 'vendorPrice',
    field: 'VendorPrice',
    header: 'Vendor Price',
    matchMode: FilterMatchMode.EQUALS
})
</script>

<template>
    <ListTable :columns="[name, vendorPrice]" :list="list">
        <Column
            :key="name.key"
            :field="name.key"
            :header="name.header"
            :sort-field="name.field"
            :filter-field="name.field"
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
            :key="vendorPrice.key"
            :field="vendorPrice.key"
            :header="vendorPrice.header"
            :sort-field="vendorPrice.field"
            :filter-field="vendorPrice.field"
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

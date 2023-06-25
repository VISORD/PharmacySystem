<script setup>
import { ref } from 'vue'
import { FilterMatchMode } from 'primevue/api'
import { list } from '@/api/pharmacy'
import ListTable from '@/components/ListTable.vue'

const name = ref({
    key: 'name',
    field: 'Name',
    header: 'Name',
    matchMode: FilterMatchMode.EQUALS
})

const address = ref({
    key: 'address',
    field: 'Address',
    header: 'Address',
    matchMode: FilterMatchMode.EQUALS
})
</script>

<template>
    <ListTable :columns="[name, address]" :list="list">
        <Column
            :key="name.key"
            :field="name.key"
            :header="name.header"
            :sort-field="name.field"
            :filter-field="name.field"
            :sortable="true"
            filter
            style="min-width: 20rem; max-width: 20rem"
            body-style="font-weight: 600"
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
            :key="address.key"
            :field="address.key"
            :header="address.header"
            :sort-field="address.field"
            :filter-field="address.field"
            :sortable="true"
            filter
            style="min-width: 40rem; max-width: 40rem"
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
            <Button type="button" icon="fa-solid fa-plus" aria-label="Add new pharmacy" />
        </template>
    </ListTable>
</template>

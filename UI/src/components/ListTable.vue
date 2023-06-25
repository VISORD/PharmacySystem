<script setup>
import { onMounted, ref } from 'vue'

const props = defineProps(['columns', 'list'])

const loading = ref(true)

const defaultFiltering = (function () {
    const dictionary = {}
    for (const item of props.columns) {
        dictionary[item.field] = { matchMode: item.matchMode }
    }

    return dictionary
})()

const filtering = ref(defaultFiltering)

const defaultOrdering = []
const ordering = ref(defaultOrdering)

const defaultPaging = { number: 0, size: 10 }
const paging = ref(defaultPaging)

const data = ref({
    items: [],
    totalAmount: 0
})

async function reload({ filters = undefined, orders = undefined, pageNumber = undefined, pageSize = undefined } = {}) {
    loading.value = true

    if (filters) {
        filtering.value = filters ?? null
    }

    const minimizedFilters = {}
    for (const y in filtering.value) {
        if (filtering.value[y].value) {
            minimizedFilters[y] = filtering.value[y]
        }
    }

    if (orders) {
        ordering.value = orders.length > 0 ? orders : []
    }

    if (pageNumber >= 0) {
        paging.value.number = pageNumber
    }

    if (pageSize >= 0) {
        paging.value.size = pageSize
    }

    const response = await props.list({
        filtering: Object.keys(minimizedFilters).length > 0 ? minimizedFilters : undefined,
        ordering: ordering.value.length > 0 ? ordering.value : undefined,
        paging: paging.value
    })

    if (response.status < 400) {
        data.value = response.data
    }

    loading.value = false
}

async function reset() {
    filtering.value = defaultFiltering
    ordering.value = defaultOrdering
    paging.value = defaultPaging
    await reload()
}

onMounted(async () => await reload())
</script>

<template>
    <DataTable
        :value="data.items"
        data-key="id"
        show-gridlines
        removable-sort
        sort-mode="multiple"
        :multi-sort-meta="ordering"
        @update:multi-sort-meta="(orders) => reload({ orders: orders })"
        filter-display="row"
        :filters="filtering"
        @update:filters="(filters) => reload({ filters: filters })"
        row-hover
        lazy
        :loading="loading"
        paginator
        :rows="paging.size"
        :total-records="data.totalAmount"
        :rows-per-page-options="[10, 20, 50, 100]"
        paginator-template="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
        current-page-report-template="{first} to {last} of {totalRecords}"
        @page="(event) => reload({ pageNumber: event.page, pageSize: event.rows })"
    >
        <template #header>
            <div style="display: flex; justify-content: space-between">
                <div>
                    <Button type="button" @click="reset()" icon="fa-solid fa-eraser" aria-label="Reset filters" />
                    <Button
                        type="button"
                        @click="reload()"
                        icon="fa-solid fa-arrows-rotate"
                        aria-label="Reload table"
                        style="margin-left: 1rem"
                    />
                </div>
                <slot name="header" />
            </div>
        </template>

        <slot />

        <template #empty>
            <div class="table-if-empty">No data.</div>
        </template>
    </DataTable>
</template>

<style scoped>
.table-if-empty {
    display: flex;
    align-items: center;
    justify-content: center;
    font-style: italic;
}
</style>

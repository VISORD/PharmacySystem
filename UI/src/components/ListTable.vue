<script setup>
import { onMounted, ref } from 'vue'

const props = defineProps(['store', 'menu'])
const contextMenu = ref()

onMounted(async () => await props.store.table.reset())
</script>

<template>
    <ContextMenu ref="contextMenu" :model="menu" style="width: 15rem" />
    <DataTable
        :value="store.table.data.items"
        data-key="id"
        show-gridlines
        removable-sort
        sort-mode="multiple"
        :multi-sort-meta="store.table.ordering"
        @update:multi-sort-meta="(orders) => store.table.reload({ orders: orders })"
        filter-display="row"
        :filters="store.table.filtering"
        @update:filters="(filters) => store.table.reload({ filters: filters })"
        row-hover
        selection-mode="single"
        :selection="store.table.selection"
        @update:selection="(selection) => store.table.selectRow(selection)"
        @row-dblclick="store.table.showInfo()"
        context-menu
        :context-menu-selection="store.table.selection"
        @update:context-menu-selection="(selection) => store.table.selectForContextMenu(selection)"
        @row-contextmenu="(event) => contextMenu.show(event.originalEvent)"
        lazy
        :loading="store.table.loading"
        paginator
        :first="store.table.paging.first"
        :rows="store.table.paging.size"
        :total-records="store.table.data.totalAmount"
        :rows-per-page-options="[10, 20, 50, 100]"
        paginator-template="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
        current-page-report-template="{first} to {last} of {totalRecords}"
        @page="(event) => store.table.reload({ pageFirst: event.first, pageNumber: event.page, pageSize: event.rows })"
    >
        <template #header>
            <div style="display: flex; justify-content: space-between">
                <div>
                    <Button
                        type="button"
                        @click="store.table.reset()"
                        icon="fa-solid fa-eraser"
                        aria-label="Reset filters"
                    />
                    <Button
                        type="button"
                        @click="store.table.reload()"
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

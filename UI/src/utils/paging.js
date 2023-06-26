import { ref } from 'vue'

export function preparePagingRequest(
    table,
    {
        filters = undefined,
        orders = undefined,
        pageFirst = undefined,
        pageNumber = undefined,
        pageSize = undefined
    } = {}
) {
    if (filters) {
        table.filtering = ref(filters ?? null)
    }

    const minimizedFilters = []
    for (const key in table.filtering) {
        if (table.filtering[key].value) {
            minimizedFilters.push({
                field: key,
                ...table.filtering[key]
            })
        }
    }

    if (orders) {
        table.ordering = ref(orders.length > 0 ? orders : [])
    }

    if (pageFirst >= 0) {
        table.paging.first = ref(pageFirst)
    }

    if (pageNumber >= 0) {
        table.paging.number = ref(pageNumber)
    }

    if (pageSize >= 0) {
        table.paging.size = ref(pageSize)
    }

    return {
        filtering: Object.keys(minimizedFilters).length > 0 ? minimizedFilters : undefined,
        ordering: table.ordering.length > 0 ? table.ordering : undefined,
        paging: table.paging
    }
}

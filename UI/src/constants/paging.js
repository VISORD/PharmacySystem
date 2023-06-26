export const defaultFiltering = (columns) => {
    const dictionary = {}
    for (const item in columns) {
        dictionary[columns[item].field] = { matchMode: columns[item].matchMode }
    }

    return dictionary
}

export const defaultOrdering = () => []

export const defaultPaging = () => ({ first: 0, number: 0, size: 10 })

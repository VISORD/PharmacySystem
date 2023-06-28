export const NO = { id: false, name: 'No' }
export const YES = { id: true, name: 'Yes' }

export const allYesNoOptions = [NO, YES]

export function resolveYesNoOption(option) {
    for (const item of allYesNoOptions) {
        if (item.id === option) {
            return item.name
        }
    }

    return null
}

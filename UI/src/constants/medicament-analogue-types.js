export const ORIGINAL = { id: false, name: 'Original' }
export const ANALOGUE = { id: true, name: 'Analogue' }

export const allMedicamentAnalogueTypes = [ORIGINAL, ANALOGUE]

export function resolveMedicamentAnalogueType(value) {
    for (const item of allMedicamentAnalogueTypes) {
        if (item.id === value) {
            return item.name
        }
    }

    return null
}

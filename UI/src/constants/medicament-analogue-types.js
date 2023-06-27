export const ORIGINAL = { id: 0, name: 'Original' }
export const ANALOGUE = { id: 1, name: 'Analogue' }

export const allMedicamentAnalogueTypes = [ORIGINAL, ANALOGUE]

export function resolveMedicamentAnalogueType(type) {
    for (const item of allMedicamentAnalogueTypes) {
        if (item.id === type) {
            return item.name
        }
    }

    return null
}

<script setup>
import MedicamentGeneralInfoView from '@/components/medicament/MedicamentGeneralInfoView.vue'
import MedicamentAnaloguesView from '@/components/medicament/MedicamentAnaloguesView.vue'
import { useMedicamentStore } from '@/stores/medicament'

const medicament = useMedicamentStore()
</script>

<template>
    <Dialog
        v-model:visible="medicament.view.dialog"
        modal
        dismissable-mask
        :header="medicament.view.profile ? `Medicament info: ${medicament.view.profile.name}` : 'Medicament info'"
        style="width: 85vw; margin: 5rem"
    >
        <TabView>
            <TabPanel header="General Info">
                <MedicamentGeneralInfoView />
            </TabPanel>
            <TabPanel header="Analogues">
                <MedicamentAnaloguesView />
            </TabPanel>
        </TabView>
    </Dialog>
</template>

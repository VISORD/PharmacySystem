<script setup>
import PharmacyGeneralInfoView from '@/components/pharmacy/PharmacyGeneralInfoView.vue'
import PharmacyMedicamentsView from '@/components/pharmacy/PharmacyMedicamentsView.vue'
import { usePharmacyStore } from '@/stores/pharmacy'

const pharmacy = usePharmacyStore()
</script>

<template>
    <Dialog
        v-model:visible="pharmacy.view.dialog"
        modal
        dismissable-mask
        :header="pharmacy.view.profile ? `Pharmacy info: ${pharmacy.view.profile.name}` : 'Pharmacy info'"
        style="width: 85vw; margin: 5rem"
    >
        <TabView>
            <TabPanel header="General Info">
                <PharmacyGeneralInfoView />
            </TabPanel>
            <TabPanel header="Medicaments">
                <PharmacyMedicamentsView />
            </TabPanel>
        </TabView>
    </Dialog>
</template>

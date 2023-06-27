<script setup>
import PharmacyMedicamentGeneralInfoView from '@/components/pharmacy/medicament/PharmacyMedicamentGeneralInfoView.vue'
import PharmacyMedicamentRatesView from '@/components/pharmacy/medicament/PharmacyMedicamentRatesView.vue'
import PharmacyMedicamentSalesView from '@/components/pharmacy/medicament/PharmacyMedicamentSalesView.vue'
import PharmacyMedicamentOrdersView from '@/components/pharmacy/medicament/PharmacyMedicamentOrdersView.vue'
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'
import { ref } from 'vue'
import router from '@/plugins/router'

const pharmacyMedicament = usePharmacyMedicamentStore()

const tab = ref(0)

async function show() {
    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, medicamentId: pharmacyMedicament.view.medicamentId }
    })

    tab.value = 0
}

async function hide() {
    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, medicamentId: undefined }
    })
}
</script>

<template>
    <Dialog
        v-model:visible="pharmacyMedicament.view.dialog"
        modal
        position="top"
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="
            pharmacyMedicament.view.profile.pharmacy && pharmacyMedicament.view.profile.medicament
                ? `Pharmacy medicament info: ${pharmacyMedicament.view.profile.pharmacy.name} — ${pharmacyMedicament.view.profile.medicament.name}`
                : 'Pharmacy medicament info'
        "
        style="width: 85vw; margin: 5rem"
    >
        <TabView class="profile-view-tab" @tab-change="(event) => (tab = event.index)">
            <TabPanel header="General Info">
                <PharmacyMedicamentGeneralInfoView v-if="tab === 0" />
            </TabPanel>
            <TabPanel header="Rates">
                <PharmacyMedicamentRatesView v-if="tab === 1" />
            </TabPanel>
            <TabPanel header="Sales">
                <PharmacyMedicamentSalesView v-if="tab === 2" />
            </TabPanel>
            <TabPanel header="Orders">
                <PharmacyMedicamentOrdersView v-if="tab === 3" />
            </TabPanel>
        </TabView>
    </Dialog>
</template>

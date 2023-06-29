<script setup>
import PharmacyGeneralInfoView from '@/components/pharmacy/PharmacyGeneralInfoView.vue'
import PharmacyMedicamentsView from '@/components/pharmacy/PharmacyMedicamentsView.vue'
import { usePharmacyStore } from '@/stores/pharmacy'
import { ref } from 'vue'
import router from '@/plugins/router'

const pharmacy = usePharmacyStore()

const tab = ref(0)

async function show() {
    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, pharmacyId: pharmacy.view.pharmacyId }
    })

    tab.value = 0
}

async function hide() {
    pharmacy.view.close()

    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, pharmacyId: undefined, pharmacyEditForm: undefined }
    })
}
</script>

<template>
    <Dialog
        v-model:visible="pharmacy.view.dialog"
        modal
        position="top"
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="pharmacy.view.profile.name ? `Pharmacy info: ${pharmacy.view.profile.name}` : 'Pharmacy info'"
        style="width: 85vw; margin: 5rem"
    >
        <TabView class="profile-view-tab" @tab-change="(event) => (tab = event.index)">
            <TabPanel header="General Info">
                <PharmacyGeneralInfoView v-if="tab === 0" />
            </TabPanel>
            <TabPanel header="Medicaments">
                <PharmacyMedicamentsView v-if="tab === 1" />
            </TabPanel>
        </TabView>
    </Dialog>
</template>

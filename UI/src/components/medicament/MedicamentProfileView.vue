<script setup>
import MedicamentGeneralInfoView from '@/components/medicament/MedicamentGeneralInfoView.vue'
import MedicamentAnaloguesView from '@/components/medicament/MedicamentAnaloguesView.vue'
import { useMedicamentStore } from '@/stores/medicament'
import { ref } from 'vue'
import router from '@/plugins/router'

const medicament = useMedicamentStore()

const tab = ref(0)

async function show() {
    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, medicamentId: medicament.view.medicamentId }
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
        v-model:visible="medicament.view.dialog"
        modal
        position="top"
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="medicament.view.profile.name ? `Medicament info: ${medicament.view.profile.name}` : 'Medicament info'"
        style="width: 85vw; margin: 5rem"
    >
        <TabView class="profile-view-tab" @tab-change="(event) => (tab = event.index)">
            <TabPanel header="General Info">
                <MedicamentGeneralInfoView v-if="tab === 0" />
            </TabPanel>
            <TabPanel header="Analogues">
                <MedicamentAnaloguesView v-if="tab === 1" />
            </TabPanel>
        </TabView>
    </Dialog>
</template>

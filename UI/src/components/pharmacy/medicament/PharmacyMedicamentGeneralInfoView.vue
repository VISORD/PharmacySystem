<script setup>
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'
import { onMounted } from 'vue'

const pharmacyMedicament = usePharmacyMedicamentStore()

onMounted(async () => await pharmacyMedicament.view.reload())
</script>

<template>
    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div style="display: flex; align-items: center; justify-content: center; height: 4rem">
                <Avatar
                    icon="fa-solid fa-hand-holding-medical"
                    size="large"
                    style="background-color: var(--text-color); color: var(--primary-color-text)"
                />
            </div>

            <Transition name="profile" mode="out-in">
                <div
                    v-if="!pharmacyMedicament.view.loading"
                    style="display: flex; align-items: center; height: 4rem; font-size: 2rem; font-weight: 700"
                >
                    {{ pharmacyMedicament.view.profile.pharmacy.name }}
                </div>
                <Skeleton v-else width="40rem" height="3rem" style="margin-bottom: 0.5rem; margin-top: 0.5rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-map-location-dot']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" style="display: flex; align-items: center">
                    {{ pharmacyMedicament.view.profile.pharmacy.address }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>
        </div>
    </div>

    <Divider />

    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div style="display: flex; align-items: center; justify-content: center; height: 4rem">
                <Avatar
                    icon="fa-solid fa-tablets"
                    size="large"
                    style="background-color: var(--text-color); color: var(--primary-color-text)"
                />
            </div>

            <Transition name="profile" mode="out-in">
                <div
                    v-if="!pharmacyMedicament.view.loading"
                    style="display: flex; align-items: center; height: 4rem; font-size: 2rem; font-weight: 700"
                >
                    {{ pharmacyMedicament.view.profile.medicament.name }}
                </div>
                <Skeleton v-else width="40rem" height="3rem" style="margin-bottom: 0.5rem; margin-top: 0.5rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-money-bill-wave']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" style="display: flex; align-items: center">
                    {{ pharmacyMedicament.view.profile.medicament.vendorPriceText }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-calculator']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" style="display: flex; align-items: center">
                    {{ pharmacyMedicament.view.profile.quantityOnHand }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>
        </div>

        <div style="display: flex; justify-content: center; width: 6rem">
            <div style="display: flex; flex-direction: column">
                <div class="profile-view-button">
                    <Button
                        icon="fa-solid fa-arrow-up-right-from-square"
                        severity="help"
                        @click="pharmacyMedicament.view.showInfo()"
                    />
                </div>
                <div class="profile-view-button">
                    <Button icon="fa-solid fa-calculator" />
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped></style>

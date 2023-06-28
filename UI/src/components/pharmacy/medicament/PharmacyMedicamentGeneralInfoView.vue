<script setup>
import { usePharmacyMedicamentStore } from '@/stores/pharmacy/medicament'
import { onMounted } from 'vue'

const pharmacyMedicament = usePharmacyMedicamentStore()

onMounted(async () => await pharmacyMedicament.view.reload())
</script>

<template>
    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div class="profile-view-header-icon">
                <Avatar icon="fa-solid fa-hand-holding-medical" size="large" class="profile-view-header-icon-avatar" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" class="profile-view-header">
                    {{ pharmacyMedicament.view.profile.pharmacy.name }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-header-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-map-location-dot']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" class="profile-view-item">
                    {{ pharmacyMedicament.view.profile.pharmacy.address }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>
        </div>
    </div>

    <Divider />

    <div style="display: flex; justify-content: space-between">
        <div class="profile-view">
            <div class="profile-view-header-icon">
                <Avatar icon="fa-solid fa-tablets" size="large" class="profile-view-header-icon-avatar" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" class="profile-view-header">
                    {{ pharmacyMedicament.view.profile.medicament.name }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-header-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-money-bill-wave']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" class="profile-view-item">
                    {{ pharmacyMedicament.view.profile.medicament.vendorPriceText }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-calculator']" />
            </div>

            <Transition name="profile" mode="out-in">
                <div v-if="!pharmacyMedicament.view.loading" class="profile-view-item">
                    {{ pharmacyMedicament.view.profile.quantityOnHand }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
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

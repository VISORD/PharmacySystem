<script setup>
import { usePharmacyStore } from '@/stores/pharmacy'
import { useConfirm } from 'primevue/useconfirm'
import { allWeekdays } from '@/constants/weekdays'
import { onMounted } from 'vue'

const confirm = useConfirm()
const pharmacy = usePharmacyStore()

const confirmDelete = () => {
    confirm.require({
        group: 'pharmacy-view-delete',
        header: 'Confirmation',
        icon: 'fa-solid fa-triangle-exclamation',
        acceptIcon: 'fa-solid fa-check',
        rejectIcon: 'fa-solid fa-xmark',
        accept: async () => await pharmacy.view.tryDelete(),
        reject: () => {}
    })
}

onMounted(async () => await pharmacy.view.reload())
</script>

<template>
    <ConfirmDialog group="pharmacy-view-delete">
        <template #message>
            <div>
                Are you sure you want to delete '<b>{{ pharmacy.view.profile.name }}</b
                >' pharmacy?
            </div>
        </template>
    </ConfirmDialog>

    <div style="display: flex; justify-content: space-between" v-if="pharmacy.view.dialog">
        <div class="profile-view">
            <div class="profile-view-header-icon">
                <Avatar icon="fa-solid fa-hand-holding-medical" size="large" class="profile-view-header-icon-avatar" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" class="profile-view-header">
                    {{ pharmacy.view.profile.name }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-header-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-at']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" class="profile-view-item">
                    {{ pharmacy.view.profile.email ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-phone']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" class="profile-view-item">
                    {{ pharmacy.view.profile.phone ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-map-location-dot']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" class="profile-view-item">
                    {{ pharmacy.view.profile.address ?? '—' }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-item-skeleton" />
            </Transition>

            <div class="profile-view-icon">
                <fa :icon="['fas', 'fa-quote-right']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" class="profile-view-item">
                    {{ pharmacy.view.profile.description ?? '—' }}
                </div>
                <Skeleton v-else width="40rem" class="profile-view-item-skeleton" />
            </Transition>
        </div>

        <div style="display: flex; justify-content: center; width: 6rem">
            <div style="display: flex; flex-direction: column">
                <div class="profile-view-button">
                    <Button
                        icon="fa-solid fa-pencil"
                        @click="pharmacy.edit.dialog = true"
                        :disabled="pharmacy.view.loading"
                    />
                </div>
                <div class="profile-view-button">
                    <Button
                        icon="fa-solid fa-trash-can"
                        severity="danger"
                        @click="confirmDelete()"
                        :disabled="pharmacy.view.loading"
                    />
                </div>
            </div>
        </div>
    </div>

    <Divider />

    <Transition name="pharmacy" mode="out-in">
        <DataTable v-if="!pharmacy.view.loading" :value="[{}]" show-gridlines table-style="width: 100%">
            <Column
                v-for="weekday of allWeekdays"
                :key="weekday.id"
                :field="weekday.id"
                style="min-width: 7rem; max-width: 7rem; height: 100px"
                :header-style="{
                    'align-items': 'center',
                    'justify-content': 'center',
                    'background-color':
                        pharmacy.view.profile.workingHours && pharmacy.view.profile.workingHours[weekday.name]
                            ? '#a1c30d'
                            : '#ea5455',
                    color: 'var(--primary-color-text)',
                    height: '25px'
                }"
            >
                <template #header>
                    <div style="text-align: center; width: 100%">
                        {{ weekday.name }}
                    </div>
                </template>
                <template #body>
                    <div style="display: flex; flex-direction: column; align-items: center; justify-content: center">
                        <div
                            v-if="
                                pharmacy.view.profile.workingHours && pharmacy.view.profile.workingHours[weekday.name]
                            "
                        >
                            {{ pharmacy.view.profile.workingHours[weekday.name].startTime }}
                        </div>
                        <div>—</div>
                        <div
                            v-if="
                                pharmacy.view.profile.workingHours && pharmacy.view.profile.workingHours[weekday.name]
                            "
                        >
                            {{ pharmacy.view.profile.workingHours[weekday.name].stopTime }}
                        </div>
                    </div>
                </template>
            </Column>
        </DataTable>
        <Skeleton v-else width="100%" height="146.5px" />
    </Transition>

    <Divider />

    <Transition name="pharmacy" mode="out-in">
        <yandex-map
            v-if="!pharmacy.view.loading && pharmacy.view.profile.latitude && pharmacy.view.profile.longitude"
            :controls="['fullscreenControl', 'geolocationControl', 'typeSelector', 'zoomControl']"
            :coords="[pharmacy.view.profile.latitude, pharmacy.view.profile.longitude]"
            zoom="17"
            style="width: 100%; height: 200px"
        >
            <ymap-marker
                marker-id="pharmacy-marker"
                marker-type="placemark"
                :icon="{ color: 'violet', glyph: 'dot' }"
                :coords="[pharmacy.view.profile.latitude, pharmacy.view.profile.longitude]"
            />
        </yandex-map>
        <Skeleton v-else width="100%" height="200px" />
    </Transition>
</template>

<style scoped></style>

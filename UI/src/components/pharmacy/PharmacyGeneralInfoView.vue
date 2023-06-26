<script setup>
import { usePharmacyStore } from '@/stores/pharmacy'
import { useConfirm } from 'primevue/useconfirm'
import { allWeekdays } from '@/constants/weekdays'

const confirm = useConfirm()
const pharmacy = usePharmacyStore()

const confirmDelete = () => {
    confirm.require({
        group: 'pharmacy-delete',
        header: 'Confirmation',
        icon: 'fa-solid fa-triangle-exclamation',
        acceptIcon: 'fa-solid fa-check',
        rejectIcon: 'fa-solid fa-xmark',
        accept: async () => await pharmacy.table.tryDelete(),
        reject: () => {}
    })
}
</script>

<template>
    <div style="display: flex; justify-content: space-between">
        <div class="pharmacy">
            <div style="display: flex; align-items: center; justify-content: center">
                <Avatar
                    icon="fa-solid fa-hand-holding-medical"
                    size="large"
                    style="background-color: var(--text-color); color: var(--primary-color-text)"
                />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div
                    v-if="!pharmacy.view.loading"
                    style="display: flex; align-items: center; height: 4rem; font-size: 2rem; font-weight: 700"
                >
                    {{ pharmacy.view.profile.name }}
                </div>
                <Skeleton v-else width="40rem" height="3rem" style="margin-bottom: 0.5rem; margin-top: 0.5rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-at']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" style="display: flex; align-items: center">
                    {{ pharmacy.view.profile.email ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-phone']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" style="display: flex; align-items: center">
                    {{ pharmacy.view.profile.phone ?? '—' }}
                </div>
                <Skeleton v-else width="20rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-map-location-dot']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" style="display: flex; align-items: center">
                    {{ pharmacy.view.profile.address ?? '—' }}
                </div>
                <Skeleton v-else width="40rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>

            <div style="display: flex; align-items: center; justify-content: center; height: 2rem">
                <fa :icon="['fas', 'fa-quote-right']" />
            </div>

            <Transition name="pharmacy" mode="out-in">
                <div v-if="!pharmacy.view.loading" style="display: flex; align-items: center">
                    {{ pharmacy.view.profile.description ?? '—' }}
                </div>
                <Skeleton v-else width="40rem" height="1.5rem" style="margin-bottom: 0.25rem; margin-top: 0.25rem" />
            </Transition>
        </div>

        <div style="display: flex; justify-content: center; width: 6rem">
            <div style="display: flex; flex-direction: column">
                <div class="profile-view-button">
                    <Button icon="fa-solid fa-pencil" />
                </div>
                <div class="profile-view-button">
                    <Button icon="fa-solid fa-trash-can" severity="danger" @click="confirmDelete()" />
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
                    'background-color': pharmacy.view.profile.workingHours[weekday.name] ? '#a1c30d' : '#ea5455',
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
                        <div v-if="pharmacy.view.profile.workingHours[weekday.name]">
                            {{ pharmacy.view.profile.workingHours[weekday.name].startTime }}
                        </div>
                        <div>—</div>
                        <div v-if="pharmacy.view.profile.workingHours[weekday.name]">
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
            v-if="!pharmacy.view.loading"
            :coords="[pharmacy.view.profile.latitude, pharmacy.view.profile.longitude]"
            zoom="17"
            style="width: 100%; height: 200px"
        >
            <ymap-marker
                marker-id="pharmacy-marker"
                marker-type="placemark"
                :coords="[pharmacy.view.profile.latitude, pharmacy.view.profile.longitude]"
            />
        </yandex-map>
        <Skeleton v-else width="100%" height="200px" />
    </Transition>
</template>

<style scoped>
.pharmacy {
    flex: 1;
    display: grid;
    grid-template-columns: 6rem fit-content(100%);
}
</style>

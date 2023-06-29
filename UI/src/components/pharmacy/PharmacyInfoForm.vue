<script setup>
import YandexMapsAddressSelector from '@/components/YandexMapsAddressSelector.vue'
import { usePharmacyStore } from '@/stores/pharmacy'
import router from '@/plugins/router'
import { useForm } from 'vee-validate'
import { ref } from 'vue'
import { addressRule, descriptionRule, emailRule, pharmacyNameRule, phoneRule } from '@/utils/validation'
import { allWeekdays } from '@/constants/weekdays'
import { formatTime, parseTime } from '@/utils/datetime'

const { handleSubmit } = useForm()
const form = ref({
    name: pharmacyNameRule(),
    email: emailRule(false),
    phone: phoneRule(),
    address: addressRule(),
    description: descriptionRule(),
    latitude: null,
    longitude: null,
    workingHours: {},
    weekDays: {}
})

for (const weekday of allWeekdays) {
    form.value.workingHours[weekday.name] = { startTime: null, stopTime: null }
    form.value.weekDays[weekday.name] = false
}

const pharmacy = usePharmacyStore()
const selector = ref()

async function show() {
    form.value.name.setValue(pharmacy.view.profile.name)
    form.value.email.setValue(pharmacy.view.profile.email)
    form.value.phone.setValue(pharmacy.view.profile.phone)
    form.value.address.setValue(pharmacy.view.profile.address)
    form.value.description.setValue(pharmacy.view.profile.description)
    form.value.latitude = pharmacy.view.pharmacyId ? pharmacy.view.profile.latitude : null
    form.value.longitude = pharmacy.view.pharmacyId ? pharmacy.view.profile.longitude : null

    for (const weekday of allWeekdays) {
        setWorkingHours(weekday.name)

        form.value.weekDays[weekday.name] =
            pharmacy.view.profile.workingHours && !!pharmacy.view.profile.workingHours[weekday.name]
    }

    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, pharmacyEditForm: true }
    })
}

async function hide() {
    form.value.name.resetField()
    form.value.email.resetField()
    form.value.phone.resetField()
    form.value.address.resetField()
    form.value.description.resetField()
    form.value.latitude = null
    form.value.longitude = null

    for (const weekday of allWeekdays) {
        form.value.workingHours[weekday.name] = { startTime: null, stopTime: null }
        form.value.weekDays[weekday.name] = false
    }

    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, pharmacyEditForm: undefined }
    })
}

function setWorkingHours(weekday, isChecked = true) {
    let startDate = null,
        stopDate = null

    if (isChecked) {
        if (pharmacy.view.profile.workingHours && pharmacy.view.profile.workingHours[weekday]) {
            if (pharmacy.view.profile.workingHours[weekday].startTime) {
                startDate = parseTime(pharmacy.view.profile.workingHours[weekday].startTime)
            }

            if (pharmacy.view.profile.workingHours[weekday].stopTime) {
                stopDate = parseTime(pharmacy.view.profile.workingHours[weekday].stopTime)
            }
        }
    }

    form.value.workingHours[weekday] = {
        startTime: startDate,
        stopTime: stopDate
    }
}

const onSubmit = handleSubmit.withControlled(async (values) => {
    const workingHours = {}
    for (const weekday in form.value.workingHours) {
        if (form.value.workingHours[weekday].startTime && form.value.workingHours[weekday].stopTime) {
            workingHours[weekday] = {
                startTime: formatTime(form.value.workingHours[weekday].startTime),
                stopTime: formatTime(form.value.workingHours[weekday].stopTime)
            }

            if (workingHours[weekday].startTime === workingHours[weekday].stopTime) {
                workingHours[weekday] = {
                    startTime: '00:00:00',
                    stopTime: '00:00:00'
                }
            }
        }
    }

    await pharmacy.edit.tryApply({
        ...values,
        latitude: form.value.latitude,
        longitude: form.value.longitude,
        workingHours: workingHours
    })
})
</script>

<template>
    <YandexMapsAddressSelector
        ref="selector"
        :coords="{
            latitude: form.latitude,
            longitude: form.longitude,
            address: form.address.value
        }"
        @apply="
            ({ latitude, longitude, address }) => {
                form.latitude = latitude
                form.longitude = longitude
                form.address.setValue(address)
            }
        "
    />

    <Dialog
        v-model:visible="pharmacy.edit.dialog"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="pharmacy.view.pharmacyId ? `Edit pharmacy info: ${pharmacy.view.profile.name}` : 'Add new pharmacy'"
        style="width: 60rem; margin: 5rem"
    >
        <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
            <form @submit="onSubmit" style="width: 100%" @keydown.enter.prevent>
                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="form-field-icon" :icon="['fas', 'hand-holding-medical']" />
                        <InputText
                            id="name"
                            v-model="form.name.value"
                            type="text"
                            placeholder="Pharmacy Name"
                            :class="{ 'p-invalid': form.name.errorMessage }"
                            autofocus
                            autocomplete="name"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.name.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="field-icon" :icon="['fas', 'at']" />
                        <InputText
                            id="email"
                            v-model="form.email.value"
                            type="text"
                            placeholder="Email"
                            :class="{ 'p-invalid': form.email.errorMessage }"
                            autocomplete="email"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.email.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="field-icon" :icon="['fas', 'phone']" />
                        <InputText
                            id="phone"
                            v-model="form.phone.value"
                            type="text"
                            placeholder="Phone"
                            :class="{ 'p-invalid': form.phone.errorMessage }"
                            autocomplete="phone"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.phone.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right" style="display: flex">
                        <Button
                            icon="fa-solid fa-location-dot"
                            style="margin-right: 1rem"
                            @click="selector.visible = true"
                        />

                        <fa class="field-icon" :icon="['fas', 'map-location-dot']" />
                        <InputText
                            id="address"
                            v-model="form.address.value"
                            type="text"
                            placeholder="Address"
                            :class="{ 'p-invalid': form.address.errorMessage }"
                            autocomplete="address"
                            disabled
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.address.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="form-field-icon" :icon="['fas', 'quote-right']" />
                        <Textarea
                            id="description"
                            v-model="form.description.value"
                            type="text"
                            placeholder="Description"
                            :class="{ 'p-invalid': form.description.errorMessage }"
                            autocomplete="description"
                            autoResize
                            rows="5"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.description.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <DataTable :value="[{}]" show-gridlines table-style="width: 100%">
                        <Column
                            v-for="weekday of allWeekdays"
                            :key="weekday.id"
                            :field="weekday.id"
                            style="min-width: 7rem; max-width: 7rem; height: 100px"
                            :header-style="{
                                'align-items': 'center',
                                'justify-content': 'center',
                                height: '25px'
                            }"
                        >
                            <template #header>
                                <div style="text-align: center; width: 100%">
                                    {{ weekday.name }}
                                </div>
                            </template>

                            <template #body>
                                <div
                                    style="
                                        display: flex;
                                        flex-direction: column;
                                        align-items: center;
                                        justify-content: center;
                                    "
                                >
                                    <InputSwitch
                                        v-model="form.weekDays[weekday.name]"
                                        @update:model-value="(value) => setWorkingHours(weekday.name, value)"
                                    />

                                    <Divider />

                                    <div>
                                        <Calendar
                                            :id="`calendar-${weekday.name}-startTime`"
                                            v-model="form.workingHours[weekday.name].startTime"
                                            time-only
                                            show-seconds
                                            :disabled="!form.weekDays[weekday.name]"
                                            placeholder="From"
                                        />
                                    </div>
                                    <div style="margin: 0.5rem"></div>
                                    <div>
                                        <Calendar
                                            :id="`calendar-${weekday.name}-stopTime`"
                                            v-model="form.workingHours[weekday.name].stopTime"
                                            timeOnly
                                            show-seconds
                                            :disabled="!form.weekDays[weekday.name]"
                                            placeholder="To"
                                        />
                                    </div>
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                </div>

                <div style="text-align: right">
                    <div class="buttons">
                        <Button label="Cancel" icon="fa-solid fa-xmark" @click="pharmacy.edit.dialog = false" text />
                        <Button label="Apply" icon="fa-solid fa-check" type="submit" />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<style scoped>
.buttons > button {
    margin: 0 0.5rem 0 0;
    width: auto;
}
</style>

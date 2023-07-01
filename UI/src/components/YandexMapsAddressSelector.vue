<script setup>
import { ref } from 'vue'
import { useField, useForm } from 'vee-validate'
import { findCoordsByAddress, watch } from '@/utils/yamaps'

const visible = ref(false)

defineExpose({ visible })
const props = defineProps(['coords'])
const emits = defineEmits(['apply'])

const map = {
    instance: null,
    marker: null,
    latitude: props.coords.latitude ?? 55.754045,
    longitude: props.coords.longitude ?? 37.613206,
    precision: null,
    suggestions: null
}

const { handleSubmit } = useForm()
const form = ref({
    address: useField('address-prompt', async function (value) {
        if (!value) {
            return 'Address is required'
        } else if (value.length > 512) {
            return 'Address length should be less or equal 512'
        }

        if (map.precision) {
            switch (map.precision) {
                case 'exact':
                    return true
                case 'number':
                case 'near':
                case 'range':
                case 'street':
                    return 'Inaccurate Address, clarify house number'
                case 'other':
                default:
                    return 'Inaccurate Address'
            }
        } else {
            return "Address isn't found"
        }
    })
})

async function show() {
    map.instance = null
    map.marker = null
    map.latitude = props.coords.latitude ?? 55.754045
    map.longitude = props.coords.longitude ?? 37.613206
    map.precision = null
    map.suggestions = null

    if (props.coords.address) {
        form.value.address.setValue(props.coords.address)
    }
}

async function hide() {
    map.suggestions?.destroy()
    form.value.address.resetField()
}

async function onMapInitialized(instance) {
    map.instance = instance
    await watch(map, form, props.coords)
}

async function onInputChange() {
    await findCoordsByAddress(map, form, form.value.address.value)
}

const onSubmit = handleSubmit.withControlled(async () => {
    emits('apply', {
        latitude: map.marker.geometry.getCoordinates()[0],
        longitude: map.marker.geometry.getCoordinates()[1],
        address: map.marker.properties.get('balloonContent')
    })

    visible.value = false
})
</script>

<template>
    <Dialog
        v-model:visible="visible"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        header="Choose the point"
        style="width: 75vw; height: 75vh"
        content-style="height: 100%"
    >
        <div class="flex flex-column" style="height: 100%">
            <yandex-map
                v-if="visible"
                :controls="['fullscreenControl', 'geolocationControl', 'typeSelector', 'zoomControl']"
                :coords="[map.latitude, map.longitude]"
                :zoom="17"
                @map-was-initialized="onMapInitialized"
                class="flex-1"
            />

            <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
                <form @submit="onSubmit" style="width: 100%" @keydown.enter.prevent>
                    <div class="field">
                        <div class="p-input-icon-right" style="display: flex">
                            <fa class="field-icon" :icon="['fas', 'map-location-dot']" />
                            <InputText
                                id="address-prompt"
                                v-model="form.address.value"
                                type="text"
                                placeholder="Address"
                                :class="{ 'p-invalid': form.address.errorMessage }"
                                autocomplete="address-prompt"
                                autofocus
                                @change="onInputChange()"
                                @keydown.enter="onInputChange()"
                            />
                        </div>
                        <small class="p-error" id="text-error">{{ form.address.errorMessage || '&nbsp;' }}</small>
                    </div>

                    <div style="text-align: right">
                        <div class="buttons">
                            <Button label="Cancel" icon="fa-solid fa-xmark" @click="visible = false" text />
                            <Button label="Apply" icon="fa-solid fa-check" type="submit" />
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </Dialog>
</template>

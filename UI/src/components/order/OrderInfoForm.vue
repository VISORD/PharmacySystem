<script setup>
import PharmacySelector from '@/components/pharmacy/PharmacySelector.vue'
import router from '@/plugins/router'
import { useField, useForm } from 'vee-validate'
import { ref } from 'vue'
import { useOrderStore } from '@/stores/order'
import { usePharmacySelectorStore } from '@/stores/pharmacy'

const { handleSubmit } = useForm()
const form = ref({
    pharmacy: useField('pharmacy', (value) => (!value ? 'Pharmacy is required' : true)),
    pharmacyId: null,
    pharmacyName: null,
    pharmacyAddress: null
})

const order = useOrderStore()
const selector = usePharmacySelectorStore()

async function show() {
    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, orderEditForm: true }
    })
}

async function hide() {
    form.value.pharmacy.resetField()
    form.value.pharmacyId = null
    form.value.pharmacyName = null
    form.value.pharmacyAddress = null

    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, orderEditForm: undefined }
    })
}

const onSubmit = handleSubmit.withControlled(
    async () => await order.edit.tryApply({ pharmacyId: form.value.pharmacyId })
)
</script>

<template>
    <PharmacySelector
        @apply="
            ({ pharmacy }) => {
                form.pharmacyId = pharmacy.id
                form.pharmacyName = pharmacy.name
                form.pharmacyAddress = pharmacy.address
                form.pharmacy.setValue(`${pharmacy.name} (${pharmacy.address})`)
            }
        "
    />

    <Dialog
        v-model:visible="order.edit.dialog"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        header="Add new order"
        class="form-dialog"
    >
        <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
            <form @submit="onSubmit" style="width: 100%" @keydown.enter.prevent>
                <div class="field">
                    <div class="p-input-icon-right" style="display: flex">
                        <Button
                            icon="fa-solid fa-arrow-pointer"
                            style="margin-right: 1rem"
                            @click="selector.table.dialog = true"
                            v-tooltip.top.hover="'Choose the pharmacy'"
                        />

                        <fa class="field-icon" :icon="['fas', 'hand-holding-medical']" />
                        <InputText
                            id="address"
                            v-model="form.pharmacy.value"
                            type="text"
                            placeholder="Pharmacy"
                            :class="{ 'p-invalid': form.pharmacy.errorMessage }"
                            autocomplete="address"
                            disabled
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.pharmacy.errorMessage || '&nbsp;' }}</small>
                </div>

                <div style="text-align: right">
                    <div class="buttons">
                        <Button label="Cancel" icon="fa-solid fa-xmark" @click="order.edit.dialog = false" text />
                        <Button label="Apply" icon="fa-solid fa-check" type="submit" />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

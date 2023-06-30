<script setup>
import MedicamentSelector from '@/components/medicament/MedicamentSelector.vue'
import { useField, useForm } from 'vee-validate'
import { ref } from 'vue'
import { useOrderStore } from '@/stores/order'
import { useOrderMedicamentStore } from '@/stores/order/medicament'
import { useMedicamentSelectorStore } from '@/stores/medicament'
import { DRAFT, ORDERED } from '@/constants/order-statuses'

const order = useOrderStore()
const orderMedicament = useOrderMedicamentStore()
const selector = useMedicamentSelectorStore()

const { handleSubmit } = useForm()
const form = ref({
    medicamentId: null,
    medicament: useField('medicament', (value) => (!value ? 'Medicament is required' : true)),
    requestedCount: useField('requestedCount', (value) => {
        if (!value) {
            return 'Requested Count is required'
        } else if (value < 0) {
            return 'Requested Count should be non-negative value'
        } else if (value > 1_000_000_000) {
            return 'Requested Count should less or equal 1 000 000 000'
        }

        return true
    }),
    approvedCount: useField('approvedCount', (value) => {
        if (!value) {
            if (order.view.profile.status === ORDERED.id) {
                return 'Approved Count is required'
            }
        } else if (value < 0) {
            return 'Approved Count should be non-negative value'
        } else if (value > 1_000_000_000) {
            return 'Approved Count should less or equal 1 000 000 000'
        } else if (orderMedicament.edit.orderMedicament.requestedCount < value) {
            return 'Approved Count should less or equal Requested Count'
        }

        return true
    })
})

function show() {
    if (orderMedicament.edit.orderMedicament) {
        form.value.medicamentId = orderMedicament.edit.orderMedicament.medicament?.id
        form.value.medicament.setValue(orderMedicament.edit.orderMedicament.medicament?.name)
        form.value.requestedCount.setValue(orderMedicament.edit.orderMedicament.requestedCount)
        form.value.approvedCount.setValue(orderMedicament.edit.orderMedicament.approvedCount)
    }
}

function hide() {
    form.value.medicamentId = null
    form.value.medicament.resetField()
    form.value.requestedCount.resetField()
    form.value.approvedCount.resetField()
    orderMedicament.edit.orderMedicament = {}
}

const onSubmit = handleSubmit.withControlled(async (values) => {
    if (order.view.profile.status === DRAFT.id) {
        await orderMedicament.edit.tryRequest({ count: values.requestedCount })
    } else {
        await orderMedicament.edit.tryApprove({ count: values.approvedCount })
    }
})
</script>

<template>
    <MedicamentSelector
        @apply="
            ({ medicament }) => {
                form.medicamentId = medicament.id
                form.medicament.setValue(medicament.name)
            }
        "
    />

    <Dialog
        v-model:visible="orderMedicament.edit.dialog"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="
            order.view.profile.status === DRAFT.id
                ? 'Request the medicament'
                : !orderMedicament.edit.orderMedicament.isApproved
                ? 'Approve the medicament'
                : 'Re-approve the medicament'
        "
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
                            v-tooltip.top.hover="'Choose the medicament'"
                            :disabled="orderMedicament.edit.orderMedicament?.id"
                        />

                        <fa class="field-icon" :icon="['fas', 'tablets']" />
                        <InputText
                            id="medicament"
                            v-model="form.medicament.value"
                            type="text"
                            placeholder="Medicament"
                            :class="{ 'p-invalid': form.medicament.errorMessage }"
                            autocomplete="medicament"
                            disabled
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.medicament.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="field-icon" :icon="['fas', 'calculator']" />
                        <InputNumber
                            id="requestedCount"
                            v-model="form.requestedCount.value"
                            placeholder="Requested Count"
                            :class="{ 'p-invalid': form.requestedCount.errorMessage }"
                            autocomplete="requestedCount"
                            :disabled="order.view.profile.status !== DRAFT.id"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.requestedCount.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="field-icon" :icon="['fas', 'calculator']" />
                        <InputNumber
                            id="approvedCount"
                            v-model="form.approvedCount.value"
                            placeholder="Approved Count"
                            :class="{ 'p-invalid': form.approvedCount.errorMessage }"
                            autocomplete="approvedCount"
                            :disabled="order.view.profile.status !== ORDERED.id"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.approvedCount.errorMessage || '&nbsp;' }}</small>
                </div>

                <div style="text-align: right">
                    <div class="buttons">
                        <Button
                            label="Cancel"
                            icon="fa-solid fa-xmark"
                            @click="orderMedicament.edit.dialog = false"
                            text
                        />

                        <Button
                            label="Apply"
                            icon="fa-solid fa-check"
                            type="submit"
                            :loading="orderMedicament.edit.processing"
                        />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<script setup>
import { useField, useForm } from 'vee-validate'
import { ref } from 'vue'
import { usePharmacyMedicamentSaleStore } from '@/stores/pharmacy/medicament/sale'

const pharmacyMedicamentSale = usePharmacyMedicamentSaleStore()

const { handleSubmit } = useForm()
const form = ref({
    soldAt: useField('soldAt', function (value) {
        if (!value) {
            return 'Sale Date/Time is required'
        }

        return true
    }),
    unitsSold: useField('unitsSold', (value) => {
        if (!value) {
            return 'Amount of Sold Units is required'
        } else if (value < 0) {
            return 'Amount of Sold Units should be non-negative value'
        } else if (value > 1_000_000_000) {
            return 'Amount of Sold Units should less or equal 1 000 000 000'
        }

        return true
    })
})

function show() {}

function hide() {
    form.value.soldAt.resetField()
    form.value.unitsSold.resetField()
    pharmacyMedicamentSale.edit.data = {}
}

const onSubmit = handleSubmit.withControlled(async (values) => await pharmacyMedicamentSale.edit.trySave(values))
</script>

<template>
    <Dialog
        v-model:visible="pharmacyMedicamentSale.edit.dialog"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        header="Add new sale"
        class="form-dialog"
        style="width: 30rem"
    >
        <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
            <form @submit="onSubmit" style="width: 100%" @keydown.enter.prevent>
                <div class="field">
                    <div class="p-input-icon-right">
                        <Calendar
                            id="soldAt"
                            v-model="form.soldAt.value"
                            date-format="dd.mm.yy"
                            show-time
                            show-seconds
                            hour-format="24"
                            placeholder="Sale Date"
                            :class="{ 'p-invalid': form.soldAt.errorMessage }"
                            autocomplete="soldAt"
                        />

                        <fa class="form-field-icon" :icon="['fas', 'calendar-day']" />
                    </div>
                    <small class="p-error" id="text-error">{{ form.soldAt.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="form-field-icon" :icon="['fas', 'calculator']" />
                        <InputNumber
                            id="unitsSold"
                            v-model="form.unitsSold.value"
                            placeholder="Units Sold"
                            :class="{ 'p-invalid': form.unitsSold.errorMessage }"
                            autocomplete="unitsSold"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.unitsSold.errorMessage || '&nbsp;' }}</small>
                </div>

                <div style="text-align: right">
                    <div class="buttons">
                        <Button
                            label="Cancel"
                            icon="fa-solid fa-xmark"
                            @click="pharmacyMedicamentSale.edit.dialog = false"
                            text
                        />

                        <Button
                            label="Apply"
                            icon="fa-solid fa-check"
                            type="submit"
                            :loading="pharmacyMedicamentSale.edit.processing"
                        />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<style scoped></style>

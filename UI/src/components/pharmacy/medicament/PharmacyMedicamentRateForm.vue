<script setup>
import { useField, useForm } from 'vee-validate'
import { ref } from 'vue'
import { usePharmacyMedicamentRateStore } from '@/stores/pharmacy/medicament/rate'
import { formatDate } from '@/utils/datetime'

const pharmacyMedicamentRate = usePharmacyMedicamentRateStore()

const { handleSubmit } = useForm()
const form = ref({
    retailPrice: useField('retailPrice', (value) => {
        if (!value) {
            return 'Retail Price is required'
        } else if (value < 0) {
            return 'Retail Price should be non-negative value'
        } else if (value > 1_000_000_000) {
            return 'Retail Price should less or equal 1 000 000 000'
        }

        return true
    }),
    startDate: useField('startDate', function (value) {
        if (!value) {
            return 'Start Date is required'
        } else if (form.value.stopDate.value < value) {
            return 'Start Date should be earlier or coincide with Stop Date'
        }

        return true
    }),
    stopDate: useField('stopDate', function (value) {
        if (!value) {
            return 'Stop Date is required'
        } else if (value < form.value.startDate.value) {
            return 'Stop Date should be later or coincide with Start Date'
        }

        return true
    })
})

function show() {
    if (pharmacyMedicamentRate.edit.data?.id) {
        form.value.retailPrice.setValue(pharmacyMedicamentRate.edit.data.retailPrice)
        form.value.startDate.setValue(new Date(Date.parse(pharmacyMedicamentRate.edit.data.startDate)))
        form.value.stopDate.setValue(new Date(Date.parse(pharmacyMedicamentRate.edit.data.stopDate)))
    }
}

function hide() {
    form.value.retailPrice.resetField()
    form.value.startDate.resetField()
    form.value.stopDate.resetField()
    pharmacyMedicamentRate.edit.data = {}
}

const onSubmit = handleSubmit.withControlled(
    async (values) =>
        await pharmacyMedicamentRate.edit.trySave({
            retailPrice: values.retailPrice,
            startDate: formatDate(values.startDate),
            stopDate: formatDate(values.stopDate)
        })
)
</script>

<template>
    <Dialog
        v-model:visible="pharmacyMedicamentRate.edit.dialog"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        header="Add new rate"
        class="form-dialog"
        style="width: 30rem"
    >
        <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
            <form @submit="onSubmit" style="width: 100%" @keydown.enter.prevent>
                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="form-field-icon" :icon="['fas', 'money-bill-wave']" />
                        <InputNumber
                            id="retailPrice"
                            v-model="form.retailPrice.value"
                            type="currency"
                            :max-fraction-digits="4"
                            placeholder="Retail Price"
                            :class="{ 'p-invalid': form.retailPrice.errorMessage }"
                            autocomplete="retailPrice"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.retailPrice.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <Calendar
                            id="startDate"
                            v-model="form.startDate.value"
                            dateFormat="dd.mm.yy"
                            placeholder="Start Date"
                            :class="{ 'p-invalid': form.startDate.errorMessage }"
                            autocomplete="startDate"
                        />

                        <fa class="form-field-icon" :icon="['fas', 'calendar-day']" />
                    </div>
                    <small class="p-error" id="text-error">{{ form.startDate.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <Calendar
                            id="stopDate"
                            v-model="form.stopDate.value"
                            dateFormat="dd.mm.yy"
                            placeholder="Stop Date"
                            :class="{ 'p-invalid': form.stopDate.errorMessage }"
                            autocomplete="stopDate"
                        />

                        <fa class="form-field-icon" :icon="['fas', 'calendar-day']" />
                    </div>
                    <small class="p-error" id="text-error">{{ form.stopDate.errorMessage || '&nbsp;' }}</small>
                </div>

                <div style="text-align: right">
                    <div class="buttons">
                        <Button
                            label="Cancel"
                            icon="fa-solid fa-xmark"
                            @click="pharmacyMedicamentRate.edit.dialog = false"
                            text
                        />

                        <Button
                            label="Apply"
                            icon="fa-solid fa-check"
                            type="submit"
                            :loading="pharmacyMedicamentRate.edit.processing"
                        />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<style scoped></style>

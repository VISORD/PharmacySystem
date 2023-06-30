<script setup>
import { useMedicamentStore } from '@/stores/medicament'
import router from '@/plugins/router'
import { useForm } from 'vee-validate'
import { ref } from 'vue'
import { descriptionRule, medicamentNameRule, medicamentVendorPriceRule } from '@/utils/validation'

const { handleSubmit } = useForm()
const form = ref({
    name: medicamentNameRule(),
    vendorPrice: medicamentVendorPriceRule(),
    description: descriptionRule()
})

const medicament = useMedicamentStore()

async function show() {
    if (medicament.view.medicamentId) {
        form.value.name.setValue(medicament.view.profile.name)
        form.value.vendorPrice.setValue(medicament.view.profile.vendorPrice)
        form.value.description.setValue(medicament.view.profile.description)
    }

    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, medicamentEditForm: true }
    })
}

async function hide() {
    form.value.name.resetField()
    form.value.vendorPrice.resetField()
    form.value.description.resetField()

    await router.push({
        path: router.currentRoute.value.path,
        query: { ...router.currentRoute.value.query, medicamentEditForm: undefined }
    })
}

const onSubmit = handleSubmit.withControlled(async (values) => await medicament.edit.tryApply(values))
</script>

<template>
    <Dialog
        v-model:visible="medicament.edit.dialog"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="
            medicament.view.medicamentId
                ? `Edit medicament info: ${medicament.view.profile.name}`
                : 'Add new medicament'
        "
        class="form-dialog"
    >
        <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
            <form @submit="onSubmit" style="width: 100%" @keydown.enter.prevent>
                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="form-field-icon" :icon="['fas', 'tablets']" />
                        <InputText
                            id="name"
                            v-model="form.name.value"
                            type="text"
                            placeholder="Medicament Name"
                            :class="{ 'p-invalid': form.name.errorMessage }"
                            autofocus
                            autocomplete="name"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.name.errorMessage || '&nbsp;' }}</small>
                </div>

                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="field-icon" :icon="['fas', 'money-bill-wave']" />
                        <InputNumber
                            id="vendorPrice"
                            v-model="form.vendorPrice.value"
                            type="currency"
                            :max-fraction-digits="4"
                            placeholder="Vendor Price"
                            :class="{ 'p-invalid': form.vendorPrice.errorMessage }"
                            autocomplete="email"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.vendorPrice.errorMessage || '&nbsp;' }}</small>
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

                <div style="text-align: right">
                    <div class="buttons">
                        <Button label="Cancel" icon="fa-solid fa-xmark" @click="medicament.edit.dialog = false" text />
                        <Button label="Apply" icon="fa-solid fa-check" type="submit" />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

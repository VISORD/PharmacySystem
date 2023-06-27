<script setup>
import { companyNameRule, emailRule, phoneRule } from '@/utils/validation'
import { useForm } from 'vee-validate'
import { ref } from 'vue'
import { useCompanyStore } from '@/stores/company'
import router from '@/plugins/router'

const { handleSubmit } = useForm()
const form = ref({
    name: companyNameRule(),
    email: emailRule(),
    phone: phoneRule()
})

const company = useCompanyStore()
company.afterLoading = function (data) {
    form.value.name.setValue(data.name)
    form.value.email.setValue(data.email)
    form.value.phone.setValue(data.phone)
}

async function show() {
    await router.push({
        fullPath: router.currentRoute.value.fullPath,
        query: { companyEditForm: true }
    })
}

async function hide() {
    await router.push({
        fullPath: router.currentRoute.value.fullPath,
        query: { companyEditForm: undefined }
    })
}

const onSubmit = handleSubmit.withControlled(async (values) => await company.tryUpdate(values))
</script>

<template>
    <Dialog
        v-model:visible="company.dialog"
        modal
        :draggable="false"
        dismissable-mask
        @show="show()"
        @hide="hide()"
        :header="company.data ? `Edit company info: ${company.data.name}` : 'Edit company info'"
        style="width: 30rem; margin: 5rem"
    >
        <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
            <form @submit="onSubmit" style="width: 100%">
                <div class="field">
                    <div class="p-input-icon-right">
                        <fa class="form-field-icon" :icon="['fas', 'users-between-lines']" />
                        <InputText
                            id="name"
                            v-model="form.name.value"
                            type="text"
                            placeholder="Company Name"
                            :class="{ 'p-invalid': form.name.errorMessage }"
                            aria-describedby="text-error"
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
                            aria-describedby="text-error"
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
                            aria-describedby="text-phone"
                            autocomplete="phone"
                        />
                    </div>
                    <small class="p-error" id="text-error">{{ form.phone.errorMessage || '&nbsp;' }}</small>
                </div>

                <div style="text-align: right">
                    <div class="buttons">
                        <Button label="Cancel" icon="fa-solid fa-xmark" @click="company.dialog = false" text />
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

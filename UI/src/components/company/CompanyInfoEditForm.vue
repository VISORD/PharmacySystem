<script setup>
import { companyNameRule, emailRule, phoneRule } from '@/constants/validation'
import { useForm } from 'vee-validate'
import { update } from '@/api/company'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'

const active = ref(false)
defineExpose({ active })

defineProps(['name', 'email', 'phone'])
const emits = defineEmits(['apply', 'update:name', 'update:email', 'update:phone'])

const { handleSubmit } = useForm()
const form = ref({
    name: companyNameRule({ model: 'name' }),
    email: emailRule({ model: 'email' }),
    phone: phoneRule({ model: 'phone' })
})

const toast = useToast()
const onSubmit = handleSubmit.withControlled(async (values) => {
    const response = await update(values)

    let notification
    if (response.status < 400) {
        active.value = false
        emits('apply')

        notification = {
            severity: 'success',
            summary: 'Company info updated',
            detail: 'The operation has been successfully performed',
            life: 3000
        }
    } else if (response.status !== 401) {
        notification = {
            severity: 'error',
            summary: 'Company info update failed',
            detail: response.data.error,
            life: 3000
        }
    }

    if (notification) {
        toast.add(notification)
    }
})
</script>

<template>
    <Dialog v-model:visible="active" modal dismissable-mask header="Edit company" style="width: 50vw">
        <div class="flex justify-content-center p-fluid" style="margin-top: 1rem">
            <form @submit="onSubmit" style="width: 95vw">
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
                        <Button label="Cancel" icon="fa-solid fa-xmark" @click="active = false" text />
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

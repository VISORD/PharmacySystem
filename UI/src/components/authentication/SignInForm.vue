<script setup>
import { useForm } from 'vee-validate'
import { emailRule, passwordRule } from '@/utils/validation'
import { useAccountStore } from '@/stores/account'
import { ref } from 'vue'

const { handleSubmit } = useForm()
const form = ref({
    email: emailRule(),
    password: passwordRule()
})

const account = useAccountStore()
const onSubmit = handleSubmit.withControlled(async (values) => await account.trySignIn(values))
</script>

<template>
    <div class="flex justify-content-center p-fluid">
        <form @submit="onSubmit" class="w-18rem">
            <div class="field">
                <div class="p-input-icon-right">
                    <fa class="form-field-icon" :icon="['fas', 'at']" />
                    <InputText
                        id="email"
                        v-model="form.email.value"
                        type="text"
                        placeholder="Email"
                        :class="{ 'p-invalid': form.email.errorMessage }"
                        aria-describedby="text-error"
                        autofocus
                        autocomplete="email"
                    />
                </div>
                <small class="p-error" id="text-error">{{ form.email.errorMessage || '&nbsp;' }}</small>
            </div>

            <div class="field">
                <Password
                    id="password"
                    input-id="password-input"
                    v-model="form.password.value"
                    placeholder="Password"
                    :class="{ 'p-invalid': form.password.errorMessage }"
                    aria-describedby="text-error"
                    toggle-mask
                    :feedback="false"
                >
                    <template #hideicon="scope">
                        <fa class="form-field-icon" :icon="['fas', 'unlock']" @click="scope.onClick()" />
                    </template>
                    <template #showicon="scope">
                        <fa class="form-field-icon" :icon="['fas', 'lock']" @click="scope.onClick()" />
                    </template>
                </Password>
                <small class="p-error" id="text-error">{{ form.password.errorMessage || '&nbsp;' }}</small>
            </div>

            <Button type="submit" label="Submit" class="mt-2" />
        </form>
    </div>
</template>

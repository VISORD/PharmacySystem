<script setup>
import { useForm } from 'vee-validate'
import { companyNameRule, emailRule, passwordRule } from '@/utils/validation'
import { useAccountStore } from '@/stores/account'
import { ref } from 'vue'

const { handleSubmit } = useForm()
const form = ref({
    email: emailRule(),
    name: companyNameRule(),
    password: passwordRule()
})

const account = useAccountStore()
const onSubmit = handleSubmit.withControlled(async (values) => await account.trySignUp(values))
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
                <div class="p-input-icon-right">
                    <fa class="form-field-icon" :icon="['fas', 'users-between-lines']" />
                    <InputText
                        id="name"
                        v-model="form.name.value"
                        type="text"
                        placeholder="Company Name"
                        :class="{ 'p-invalid': form.name.errorMessage }"
                        aria-describedby="text-error"
                        autocomplete="name"
                    />
                </div>
                <small class="p-error" id="text-error">{{ form.name.errorMessage || '&nbsp;' }}</small>
            </div>

            <div class="field">
                <Password
                    id="password"
                    input-id="password-input"
                    v-model="form.password"
                    placeholder="Password"
                    :class="{ 'p-invalid': form.password.errorMessage }"
                    aria-describedby="text-error"
                    toggle-mask
                >
                    <template #header>
                        <div style="padding-top: 20px" />
                    </template>
                    <template #footer>
                        <Divider />
                        <p class="mt-2">Suggestions</p>
                        <div style="padding-left: 10px">
                            <ul class="pl-2 ml-2 mt-0" style="line-height: 1.5">
                                <li>At least one lowercase</li>
                                <li>At least one uppercase</li>
                                <li>At least one numeric</li>
                                <li>Minimum 8 characters</li>
                            </ul>
                        </div>
                    </template>
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

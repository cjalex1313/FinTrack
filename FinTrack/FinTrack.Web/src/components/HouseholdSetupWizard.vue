<template>
  <div class="household-setup-wizard pt-12 max-w-5xl mx-auto">
    <div class="mb-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-2">Household Setup</h1>
      <p class="text-gray-600">Let's set up your household financial tracking in 3 simple steps.</p>
    </div>

    <Stepper v-model:value="activeStep" class="basis-50rem">
      <StepList>
        <Step value="1">Household Information</Step>
        <Step value="2">Income Sources</Step>
        <Step value="3">Expense Categories</Step>
      </StepList>

      <StepPanels>
        <StepPanel value="1">
          <div class="flex flex-col">
            <div class="border-2 border-dashed border-gray-300 rounded-lg p-8 text-center">
              <h2 class="text-xl font-semibold text-gray-800 mb-2">
                Let's give your household a name
              </h2>
              <p class="text-gray-600 mb-8">
                This will be the main identifier for your financial plan. You can use your family
                name (e.g., "The Miller Household") or your address ("123 Maple St").
              </p>
              <div class="flex justify-center">
                <FloatLabel variant="on">
                  <InputText id="householdName" v-model="household.name" class="w-full md:w-80" />
                  <label for="householdName">Household Name</label>
                </FloatLabel>
              </div>
            </div>
          </div>
          <div class="flex pt-6 justify-end">
            <Button
              label="Next"
              icon="pi pi-arrow-right"
              iconPos="right"
              @click="nextStep"
              :disabled="!household.name"
            />
          </div>
        </StepPanel>

        <StepPanel value="2">
          <div class="flex flex-col">
            <div
              class="border-2 border-dashed border-gray-300 rounded-lg flex-1 flex items-center justify-center"
            >
              <div class="text-center">
                <i class="pi pi-dollar text-4xl text-gray-400 mb-4"></i>
                <p class="text-gray-500">Income sources will go here</p>
                <p class="text-sm text-gray-400 mt-2">Salary, freelance, investments, etc.</p>
              </div>
            </div>
          </div>
          <div class="flex pt-6 justify-between">
            <Button label="Back" icon="pi pi-arrow-left" severity="secondary" @click="prevStep" />
            <Button label="Next" icon="pi pi-arrow-right" iconPos="right" @click="nextStep" />
          </div>
        </StepPanel>

        <StepPanel value="3">
          <div class="flex flex-col">
            <div
              class="border-2 border-dashed border-gray-300 rounded-lg flex-1 flex items-center justify-center"
            >
              <div class="text-center">
                <i class="pi pi-tags text-4xl text-gray-400 mb-4"></i>
                <p class="text-gray-500">Expense categories will go here</p>
                <p class="text-sm text-gray-400 mt-2">
                  Food, transport, utilities, entertainment, etc.
                </p>
              </div>
            </div>
          </div>
          <div class="flex pt-6 justify-between">
            <Button label="Back" icon="pi pi-arrow-left" severity="secondary" @click="prevStep" />
            <Button
              label="Complete Setup"
              icon="pi pi-check"
              iconPos="right"
              severity="success"
              @click="completeSetup"
            />
          </div>
        </StepPanel>
      </StepPanels>
    </Stepper>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Stepper from 'primevue/stepper'
import StepList from 'primevue/steplist'
import Step from 'primevue/step'
import StepPanels from 'primevue/steppanels'
import StepPanel from 'primevue/steppanel'
import Button from 'primevue/button'
import { FloatLabel, InputText } from 'primevue'
import { EMPTY_GUID, type HouseholdDTO, type SetupDTO } from '@/api/models'
import { useHouseholdApi } from '@/api/householdApi'

const houstholdApi = useHouseholdApi()

const emit = defineEmits<{
  (e: 'completed'): void
}>()

const activeStep = ref('1')
const household = ref<HouseholdDTO>({
  id: EMPTY_GUID,
  name: '',
})

const nextStep = () => {
  const currentStep = parseInt(activeStep.value)
  if (currentStep < 3) {
    activeStep.value = (currentStep + 1).toString()
    console.log(`Moving to step ${activeStep.value}`)
  }
}

const prevStep = () => {
  const currentStep = parseInt(activeStep.value)
  if (currentStep > 1) {
    activeStep.value = (currentStep - 1).toString()
    console.log(`Moving to step ${activeStep.value}`)
  }
}

const completeSetup = async () => {
  const setupDto: SetupDTO = {
    household: household.value,
    recurringIncomes: [],
  }
  await houstholdApi.setupHousehold(setupDto)

  emit('completed')
}
</script>

<style scoped>
/* Custom styling for the stepper content areas */
</style>

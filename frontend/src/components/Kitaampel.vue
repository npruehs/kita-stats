<template>
  <v-container class="fill-height" max-width="900">
    <v-row align="center" justify="center">
      <v-col cols="12" class="text-center mb-6">
        <h1 class="text-h4 font-weight-bold">Kita-Ampel</h1>
      </v-col>

      <v-col cols="12">
        <v-select
          v-model="selectedCompany"
          :items="companyOptions"
          label="Träger"
          item-title="label"
          item-value="value"
          outlined
        />
      </v-col>

      <v-col cols="12">
        <v-select
          v-model="selectedKita"
          :items="kitaOptions"
          label="Kita"
          item-title="label"
          item-value="value"
          outlined
          :disabled="!selectedCompany"
        />
      </v-col>

      <v-col cols="12">
        <v-select
          v-model="selectedCareLevel"
          :items="careLevelOptions"
          label="Betreuung"
          item-title="label"
          item-value="value"
          outlined
        />
      </v-col>

      <v-col cols="12" class="text-center mt-4">
        <v-btn
          color="primary"
          @click="onSubmit"
          :disabled="!selectedCompany || !selectedKita || !selectedCareLevel"
        >
          Abschicken
        </v-btn>
      </v-col>

      <v-snackbar v-model="showSnackbar" location="bottom" timeout="3000">
        Meldung erfolgreich erfasst!
        <template #actions>
          <v-btn variant="text" color="white" @click="showSnackbar = false">OK</v-btn>
        </template>
      </v-snackbar>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

// Current values
const selectedCompany = ref<string | null>(null)
const selectedKita = ref<string | null>(null)
const selectedCareLevel = ref<string | null>(null)

const showSnackbar = ref(false)

// Static data
const companyOptions = [
  { label: 'Ballin', value: 'Ballin' },
  { label: 'Elbkinder', value: 'Elbkinder' },
  { label: 'Kinderzimmer', value: 'Kinderzimmer' },
]

const kitasByCompany: Record<string, Array<{ label: string; value: string }>> = {
  Ballin: [
    { label: 'Ballin Kita mit Eltern-Kind-Zentrum Boberg', value: 'Ballin Kita mit Eltern-Kind-Zentrum Boberg' },
    { label: 'Ballin Kita mit Eltern-Kind-Zentrum Leuchtturm', value: 'Ballin Kita mit Eltern-Kind-Zentrum Leuchtturm' },
  ],
  Elbkinder: [
    { label: 'Elbkinder-Kita Mendelstraße', value: 'Elbkinder-Kita Mendelstraße' },
    { label: 'Elbkinder-Kita Weidemoor', value: 'Elbkinder-Kita Weidemoor' },
  ],
  Kinderzimmer: [
    { label: 'Kita kinderzimmer Tienrade', value: 'Kita kinderzimmer Tienrade' },
    { label: 'Kita kinderzimmer Schilfpark', value: 'Kita kinderzimmer Schilfpark' },
  ],
}

const careLevelOptions = [
  { label: 'gelb (Betreuungsangebot eingeschränkt)', value: 'yellow' },
  { label: 'orange (min. eine Gruppe geschlossen)', value: 'orange' },
  { label: 'rot (Einrichtung geschlossen)', value: 'red' },
]

// UI logic
const kitaOptions = computed(() => {
  if (!selectedCompany.value) return []
  return kitasByCompany[selectedCompany.value] ?? []
})

watch(selectedCompany, () => {
  // Clear selected Kita when company changes
  selectedKita.value = null
})

// Handlers
function onSubmit() {
  // Placeholder for future submit logic — show success message for now
  showSnackbar.value = true
}
</script>

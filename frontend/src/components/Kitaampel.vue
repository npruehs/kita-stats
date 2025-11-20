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
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

const companyOptions = [
  { label: 'Ballin', value: 'Ballin' },
  { label: 'Elbkinder', value: 'Elbkinder' },
  { label: 'Kinderzimmer', value: 'Kinderzimmer' },
]

const selectedCompany = ref<string | null>(null)
const selectedKita = ref<string | null>(null)

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

const kitaOptions = computed(() => {
  if (!selectedCompany.value) return []
  return kitasByCompany[selectedCompany.value] ?? []
})

watch(selectedCompany, () => {
  // Clear selected Kita when company changes
  selectedKita.value = null
})

const careLevelOptions = [
  { label: 'gelb (Betreuungsangebot eingeschränkt)', value: 'yellow' },
  { label: 'orange (min. eine Gruppe geschlossen)', value: 'orange' },
  { label: 'rot (Einrichtung geschlossen)', value: 'red' },
]

const selectedCareLevel = ref<string | null>(null)
</script>

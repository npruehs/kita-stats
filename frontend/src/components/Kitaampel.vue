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

      <v-snackbar v-model="showSuccess" location="bottom" timeout="3000">
        Meldung erfolgreich erfasst!
        <template #actions>
          <v-btn variant="text" color="white" @click="showSuccess = false">OK</v-btn>
        </template>
      </v-snackbar>
      <v-snackbar v-model="showError" color="red" location="bottom" timeout="4000">
        Fehler beim Senden der Meldung!
        <template #actions>
          <v-btn variant="text" color="white" @click="showError = false">OK</v-btn>
        </template>
      </v-snackbar>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

// UUID generator
function getOrCreateUserId() {
  const key = 'kitastats-user-id';
  let userId = localStorage.getItem(key);
  if (!userId) {
    userId = crypto.randomUUID();
    localStorage.setItem(key, userId);
  }
  return userId;
}

// Current values
const selectedCompany = ref<string | null>(null)
const selectedKita = ref<string | null>(null)
const selectedCareLevel = ref<string | null>(null)

const showSuccess = ref(false)
const showError = ref(false)

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
  { label: 'gelb (Betreuungsangebot eingeschränkt)', value: '1' },
  { label: 'orange (min. eine Gruppe geschlossen)', value: '2' },
  { label: 'rot (Einrichtung geschlossen)', value: '3' },
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
  const userId = getOrCreateUserId();
  const payload = {
    userId,
    company: selectedCompany.value,
    kita: selectedKita.value,
    careLevel: selectedCareLevel.value,
  };
  fetch('http://localhost:7071/api/UpdateCareLevel', {
  //fetch('https://kitastats-adagcbcubec5bzhz.germanywestcentral-01.azurewebsites.net/api/UpdateCareLevel', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
    .then((res) => {
      if (!res.ok) throw new Error('Network error');
      showSuccess.value = true;
    })
    .catch(() => {
      showError.value = true;
    });
}
</script>

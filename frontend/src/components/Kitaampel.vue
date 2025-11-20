<template>
  <v-container class="fill-height" max-width="900">
    <v-row align="center" justify="center">
      <v-col cols="12" class="text-center mb-6">
        <h1 class="text-h4 font-weight-bold">Kita-Ampel</h1>
      </v-col>

      <v-col cols="12" md="6">
        <v-select
          v-model="selectedCompany"
          :items="companyOptions"
          label="Träger"
          item-title="label"
          item-value="value"
          outlined
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const companyOptions = [
  { label: 'Ballin', value: 'Ballin' },
  { label: 'Elbkinder', value: 'Elbkinder' },
  { label: 'kinderzimmer', value: 'Kinderzimmer' },
]

const selectedCompany = ref<string | null>(null)
</script>

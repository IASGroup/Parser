<script setup lang="ts">
import {ref} from "vue";
import _ from "lodash"

const menuTaskTypes = [
  "Задача парсинга АПИ",
  "Задача парсинга текста сайта",
  "Задача парсинга тегов сайта"
];

const selectedMenuTaskType = ref<string>("Задача парсинга АПИ");

enum PartTypes {
  Path = 0,
  Query = 1
}

type UrlPart = {
  name: string,
  valueOptions: {
    range: {
      start: number,
      end: number
    } | null,
    values: Array<{
      value: string
    }> | null,
    value: string | null
  },
  partType: PartTypes
}
const urlParts = ref<Array<UrlPart>>([])

const valueOptionsMenu = [
  "Range",
  "Values",
  "Value"
];
const urlInputEventHandler = _.debounce(e => urlInputValueChangedHandler(e.target.value), 300);

function urlInputValueChangedHandler(newUrlInputValue: string): void {
  const queryStartIndex = newUrlInputValue.indexOf("?");
  const urlWithoutQueries = queryStartIndex === -1 ? newUrlInputValue : newUrlInputValue.slice(0, queryStartIndex);
  const matches = [...urlWithoutQueries.matchAll(new RegExp("{\\w+}", "g"))];
  urlParts.value = matches.map<UrlPart>(x => {
    const match = x[0];
    return {
      name: match.slice(1).slice(0, -1),
      valueOptions: {range: null, values: null, value: match},
      partType: PartTypes.Path
    }
  });
}
</script>

<template>
  <v-container>
    <v-card class="d-flex flex-column pa-5" style="min-height: 70vh">
      <div class="d-flex justify-space-between">
        <v-card-title class="text-primary ma-0 pa-0">Создание задачи парсинга</v-card-title>
        <div style="width: 300px">
          <v-select color="primary" variant="solo" v-model="selectedMenuTaskType" :items="menuTaskTypes"/>
        </div>
      </div>
      <v-container v-if="selectedMenuTaskType === menuTaskTypes[0]">
        <v-text-field label="Url" variant="solo" v-on:keyup="e => urlInputEventHandler(e)"/>
        <v-card>
          <v-card-title class="text-subtitle-1">Path params</v-card-title>
          <v-list>
            <v-list-item v-for="urlPart in urlParts" :key="urlPart.name">
              <v-row>
                <v-col>
                  {{ urlPart.name }}
                </v-col>
                <v-col>
                  <v-select variant="solo" :items="valueOptionsMenu"></v-select>
                </v-col>
              </v-row>
              <v-row>

              </v-row>
            </v-list-item>
          </v-list>
        </v-card>
      </v-container>
      <v-container v-else-if="selectedMenuTaskType === menuTaskTypes[1]">
        Test 2
      </v-container>
      <v-container v-else>
        Test 3
      </v-container>
      <v-spacer/>
      <v-card-actions class="pa-0 ma-0">
        <v-btn block="true" color="primary" variant="flat">Создать</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

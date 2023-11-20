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
  Path = "path",
  Query = "query"
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
  partType: PartTypes,
  selectedValueOption: string,
  valueOptionInputError: string | null,
  valueOptionInputSuccess: boolean,
}
const urlParts = ref<Array<UrlPart>>([])

const valueOptionsMenu = [
  "Range",
  "Values",
  "Value"
];
const taskName = ref<string | null>(null);

const taskNameKeyUpEventHandler = _.debounce((event) => taskNameInputValueChangedHandler(event.target.value), 300);
function taskNameInputValueChangedHandler(newTaskName: string) {
  taskName.value = newTaskName;
}

const urlKeyUpEventHandler = _.debounce(e => urlInputValueChangedHandler(e.target.value), 300);
function urlInputValueChangedHandler(newUrlInputValue: string): void {
  const queryStartIndex = newUrlInputValue.indexOf("?");
  const urlWithoutQueries = queryStartIndex === -1 ? newUrlInputValue : newUrlInputValue.slice(0, queryStartIndex);
  const matches = [...urlWithoutQueries.matchAll(new RegExp("{\\w+}", "g"))];
  const paths = matches.map<UrlPart>(x => {
    const match = x[0];
    const name = match.slice(1).slice(0, -1);
    const previousUrlPart = urlParts.value.filter(x => x.name === name);
    return {
      name: name,
      valueOptions: {
        range: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptions.range : null,
        values: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptions.values : null,
        value: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptions.value: null
      },
      partType: PartTypes.Path,
      selectedValueOption: valueOptionsMenu[2],
      valueOptionInputError: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptionInputError : null,
      valueOptionInputSuccess: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptionInputSuccess : false,
    }
  });
  const queries: Array<UrlPart> = [];
  const queriesString = queryStartIndex === -1 || queryStartIndex === newUrlInputValue.length - 1 ? '' : newUrlInputValue.slice(queryStartIndex + 1);
  if (queriesString.length != 0) {
    const queriesSplit = queriesString.split('&').filter(x => x !== '');
    const queriesFromSplit = queriesSplit.map<UrlPart>(x => {
      const queryName = x.split('=')[0];
      const previousUrlPart = urlParts.value.filter(x => x.name === queryName);
      return {
        name: queryName,
        valueOptions: {
          range: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptions.range : null,
          values: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptions.values : null,
          value: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptions.value: null
        },
        partType: PartTypes.Query,
        selectedValueOption: valueOptionsMenu[2],
        valueOptionInputError: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptionInputError : null,
        valueOptionInputSuccess: previousUrlPart.length !== 0 ? previousUrlPart[0].valueOptionInputSuccess : false,
      }
    })
    queries.push(...queriesFromSplit);
  }
  urlParts.value = paths.concat(queries);
}

const valueOptionsKeyUpEventHandler = _.debounce((event, urlPartName: string) => valueOptionsInputValueChangedHandler(event.target.value, urlPartName), 300);
function valueOptionsInputValueChangedHandler(newInputValue: string, urlPartName: string) {
  const urlPart = urlParts.value.filter(x => x.name === urlPartName)[0];
  urlPart.valueOptionInputSuccess = false;
  urlPart.valueOptions.value = null;
  urlPart.valueOptions.values = null;
  urlPart.valueOptions.range = null;
  const firstKeyWordMatch = newInputValue.match(new RegExp("(range|values|value)"));
  const firstKeyWord = firstKeyWordMatch ? firstKeyWordMatch[0].replaceAll(' ', '') : null;
  const firstKeyWordCorrect = firstKeyWord && (firstKeyWord === "range" || firstKeyWord === "values" || firstKeyWord === "value")
  if (!firstKeyWordCorrect) {
    urlPart.valueOptionInputError = "Значение должно начинаться с range или values или value"
    return;
  }
  const inputWithoutFirstKeyWord = newInputValue.replace(firstKeyWord!, '').trim();
  const valuesSplit = inputWithoutFirstKeyWord.split(',');
  if (firstKeyWord === "range" || firstKeyWord === "values") {
    if (valuesSplit[0][0] !== '[' || valuesSplit[valuesSplit.length - 1].slice(-1) !== ']') {
      urlPart.valueOptionInputError = "Значения должны быть заключены в скобки: [1,2,3]"
      return;
    }
    if (valuesSplit.length < 2) {
      urlPart.valueOptionInputError = "Необходимо ввести как минимум два значения: [1, 2]"
      return;
    }

    if (valuesSplit[0].trim().length < 2 || valuesSplit[valuesSplit.length - 1].trim().length < 2){
      urlPart.valueOptionInputError = "Необходимо корректно ввести первое и последнее значение: [1,2]"
      return;
    }

    if (firstKeyWord === "range") {
      if (valuesSplit.length !== 2) {
        urlPart.valueOptionInputError = "Для range допустимо ввести только 2 значения"
        return;
      }
      const firstValue = parseInt(valuesSplit[0].trim().slice(1));
      const secondValue = parseInt(valuesSplit[1].trim().slice(0,-1));
      if (isNaN(firstValue) || isNaN(secondValue)) {
        urlPart.valueOptionInputError = "Для range допустимы только целочисленные значения"
        return;
      }
      urlPart.valueOptions.range = { start: firstValue, end: secondValue };
    }
    if (firstKeyWord == "values") {
      const firstValue = valuesSplit[0].trim().slice(1);
      const lastValue = valuesSplit.slice(-1)[0].trim().slice(0,-1);
      const values = [firstValue].concat(valuesSplit.slice(1, -1).map(x => x.trim())).concat([lastValue]);
      urlPart.valueOptions.values = values.map(x => { return { value: x }});
    }
  }
  if (firstKeyWord == "value") {
    if (inputWithoutFirstKeyWord.trim().length === 0) {
      urlPart.valueOptionInputError = "Необходимо ввести значение"
      return;
    }
    urlPart.valueOptions.value = inputWithoutFirstKeyWord.trim();
  }
  urlPart.valueOptionInputError = null;
  urlPart.valueOptionInputSuccess = true;
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
        <v-text-field
          variant="solo"
          label="Название задачи"
          v-on:keyup="e => taskNameKeyUpEventHandler(e)"
        />
        <v-text-field
          variant="solo" v-on:keyup="e => urlKeyUpEventHandler(e)"
          placeholder="https://localhost:3000/users/{user}?todo&part"
          hint="Используйте {param} для установки параметров в пути url"
          class="mb-3"
        />
        <v-card v-if="urlParts.length !== 0">
          <v-card-title class="text-subtitle-1">Url params</v-card-title>
          <v-list>
            <v-list-item v-for="urlPart in urlParts" :key="urlPart.name">
             <v-card>
               <v-row class="ma-0 pa-1 d-flex align-center">
                 <v-col cols="2" class="d-flex justify-center pa-0">
                   <v-badge
                     offset-y="-15" location="left top" :content="urlPart.partType"
                     :color="urlPart.partType === PartTypes.Path ? 'blue-darken-2' : 'orange-darken-2'"
                   >
                     <span class="text-wrap">{{urlPart.name}}</span>
                   </v-badge>
                 </v-col>
                 <v-col class="d-flex justify-center align-center pa-0">
                   <v-text-field
                     :error="!!urlPart.valueOptionInputError"
                     :error-messages="urlPart.valueOptionInputError"
                     variant="solo"
                     placeholder="type range [start, end] or values [one, two ..] or value 2"
                     v-on:keyup="e => valueOptionsKeyUpEventHandler(e, urlPart.name)"
                     style="margin-top: 22px"
                   />
                   <v-icon v-if="urlPart.valueOptionInputSuccess" icon="mdi-check-bold" class="ml-4" color="green"/>
                 </v-col>
               </v-row>
             </v-card>
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
        <v-btn :block="true" color="primary" variant="flat">Создать</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

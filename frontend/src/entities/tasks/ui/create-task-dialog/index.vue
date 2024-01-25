<script setup lang="ts">
import {ref} from "vue";
import _ from "lodash"
import { CreateTaskRequest, CreateTaskAsync } from "@/shared/api";
import {storeToRefs} from "pinia";
import {taskStore} from "@/entities/tasks";

const emit = defineEmits(['parserTaskCreated']);

const {tasks} = storeToRefs(taskStore());

const menuTaskTypes = [
  "Задача парсинга АПИ",
  "Задача парсинга текста сайта",
  "Задача парсинга тегов сайта"
];

const httpRequestMethods = [
  "GET",
  "POST",
  "PUT",
  "DELETE"
]

const selectedHttpRequestMethod = ref<string>(httpRequestMethods[0]);

const postMethodBody = ref<string | null>(null);

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

const taskUrl = ref<string | null>(null);

const taskName = ref<string | null>(null);

const headers = ref<Array<{
  name: string,
  value: string
}>>([])

const currentAddedHeader = ref<{
  name: string | null,
  value: string | null,
  error: string | null
}>({
  name: null,
  value: null,
  error: null
})

const showAddHeaderDialog = ref<boolean>(false);

const tags = ref<Array<{
  name: string,
  attributes: Array<{
    name: string,
    value: string
  }>
}>>([]);

const showAddTagDialog = ref<boolean>(false);

const currentAddedTag = ref<{
  name: string | null,
  attributes: Array<{
    name: string,
    value: string
  }>,
  error: string | null
}>({
  name: null,
  attributes: [],
  error: null
});

const showAddTagAttributeDialog = ref<boolean>(false);

const currentAddedAttribute = ref<{
  name: string | null,
  value: string | null,
  error: string | null,
}>({
  name: null,
  value: null,
  error: null
})

const loadingCreateTask = ref<boolean>(false);

function addTagAttribute() {
  if (!currentAddedAttribute.value.name || !currentAddedAttribute.value.value) {
    currentAddedAttribute.value.error = "Значение не может быть пустым";
    return;
  }
  currentAddedTag.value.attributes.push({
    name: currentAddedAttribute.value.name,
    value: currentAddedAttribute.value.value
  });
  currentAddedAttribute.value = {
    name: null,
    value: null,
    error: null
  }
  showAddTagAttributeDialog.value = false;
}

function removeTagAttribute(index: number) {
  currentAddedTag.value.attributes = currentAddedTag.value.attributes.filter(x => x !== currentAddedTag.value.attributes[index]);
}

function addTag() {
  if (!currentAddedTag.value.name) {
    currentAddedTag.value.error = "Название не может быть пустым";
    return;
  }
  tags.value.push({
    name: currentAddedTag.value.name,
    attributes: currentAddedTag.value.attributes
  });
  currentAddedTag.value = {
    name: null,
    attributes: [],
    error: null
  };
  showAddTagDialog.value = false;
}

function removeTag(index: number) {
  tags.value = tags.value.filter(x => x !== tags.value[index]);
}

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
  taskUrl.value = newUrlInputValue;
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

function addHeader() {
  if (!currentAddedHeader.value.name || !currentAddedHeader.value.value){
    currentAddedHeader.value.error = "Значение не может быть пустым";
    return;
  }
  headers.value.push({
    name: currentAddedHeader.value.name,
    value: currentAddedHeader.value.value
  });
  currentAddedHeader.value = {
    name: null,
    value: null,
    error: null
  };
  showAddHeaderDialog.value = false;
}

function removeHeader(index: number) {
  headers.value = headers.value.filter(x => x !== headers.value[index]);
}

async function createParserTask() : Promise<void> {
  loadingCreateTask.value = true;
  const createTask : CreateTaskRequest = {
    name: taskName.value as string,
    url: taskUrl.value as string,
    typeId: menuTaskTypes.indexOf(selectedMenuTaskType.value) + 1,
    parserTaskUrlOptions: {
      requestMethod: selectedHttpRequestMethod.value,
      postMethodOptions: selectedHttpRequestMethod.value === "POST" ? {
        requestBody:  postMethodBody.value as string
      } : null,
      paths: urlParts.value.filter(x => x.partType === PartTypes.Path).map(x => {
        return {
          name: x.name,
          valueOptions: x.valueOptions
        }
      }),
      queries: urlParts.value.filter(x => x.partType === PartTypes.Query).map(x => {
        return {
          name: x.name,
          valueOptions: x.valueOptions
        }
      }),
      headers: headers.value as Array<{
        name: string,
        value: string
      }>
    },
    parserTaskWebsiteTagsOptions: tags.value.length === 0
      ? null
      : {
        parserTaskWebsiteTags: tags.value.map(x => {
          return {
            findOptions: x
          }
        })
      }
  }
  const response = await CreateTaskAsync(createTask);
  if (!response.isSuccess) {
    console.log(response);
  }
  loadingCreateTask.value = false;
  tasks.value.push({
    name: response.result?.name as string,
    statusId: response.result?.statusId as number,
    url: response.result?.url as string,
    id: response.result?.id as string,
    typeId: response.result?.typeId as number,
    hasError: false,
    allPartsNumber: response.result?.allPartsNumber as number,
    completedPartsNumber: 0
  });
  emit('parserTaskCreated');
}
</script>

<template>
  <v-dialog v-model="showAddHeaderDialog" class="w-75">
    <v-card class="pa-5">
      <v-card-title class="text-primary ma-0 pa-0">Добавление заголовка</v-card-title>
      <div class="d-flex flex-row align-center mt-4">
        <v-text-field :error="currentAddedHeader.error !== null" hide-details variant="solo" label="Название" v-model="currentAddedHeader.name"/>
        <v-text-field :error="currentAddedHeader.error !== null" hide-details variant="solo" label="Значение" class="ml-6" v-model="currentAddedHeader.value"/>
        <v-btn @click="addHeader" class="ml-6" color="primary">Добавить</v-btn>
      </div>
    </v-card>
  </v-dialog>
  <v-dialog v-model="showAddTagDialog" class="w-75">
    <v-card class="pa-5">
      <v-text-field :error="currentAddedTag.error !== null" variant="solo" class="mb-4" hide-details label="Название тега" v-model="currentAddedTag.name" />
      <v-card v-if="currentAddedTag.attributes.length !== 0" :flat="true" class="mb-6">
        <v-card-title class="mb-3">Аттрибуты поиска тега</v-card-title>
        <v-list class="ma-0 pa-0">
          <v-list-item class="ma-0 pa-0" v-for="(attribute, index) in currentAddedTag.attributes" :key="index">
            <v-row class="ma-0 pa-0 d-flex align-center">
              <v-col class="pa-1 ma-0">
                <v-text-field hide-details label="Название" :readonly="true" persistent-placeholder variant="solo">{{attribute.name}}</v-text-field>
              </v-col>
              <v-col class="pa-1 ma-0">
                <v-text-field hide-details label="Значение" :readonly="true" persistent-placeholder variant="solo">{{attribute.value}}</v-text-field>
              </v-col>
              <v-col cols="2" class="d-flex justify-center">
                <v-btn @click="removeTagAttribute(index)" color="red">
                  <v-icon>mdi-close</v-icon>
                </v-btn>
                <v-btn class="ml-4" @click="showAddTagAttributeDialog = true" color="primary">
                  <v-icon>mdi-plus</v-icon>
                </v-btn>
              </v-col>
            </v-row>
          </v-list-item>
        </v-list>
      </v-card>
      <v-btn v-if="currentAddedTag.attributes.length === 0" @click="showAddTagAttributeDialog = true" color="primary" class="mb-4">Добавить поиск по атрибутам</v-btn>
      <v-btn @click="addTag" color="primary">Добавить тег</v-btn>
    </v-card>
  </v-dialog>
  <v-dialog v-model="showAddTagAttributeDialog" class="w-75">
    <v-card class="pa-5">
      <v-card-title class="text-primary ma-0 pa-0">Добавление атрибута</v-card-title>
      <div class="d-flex flex-row align-center mt-4">
        <v-text-field :error="currentAddedAttribute.error !== null" hide-details variant="solo" label="Название" v-model="currentAddedAttribute.name"/>
        <v-text-field :error="currentAddedAttribute.error !== null" hide-details variant="solo" label="Значение" class="ml-6" v-model="currentAddedAttribute.value"/>
        <v-btn @click="addTagAttribute" class="ml-6" color="primary">Добавить</v-btn>
      </div>
    </v-card>
  </v-dialog>
  <v-container>
    <v-card class="d-flex flex-column pa-5" style="min-height: 70vh; max-height: 90vh; overflow-y: auto">
      <div class="d-flex justify-space-between">
        <v-card-title class="text-primary ma-0 pa-0">Создание задачи парсинга</v-card-title>
        <div style="width: 300px">
          <v-select color="primary" variant="solo" v-model="selectedMenuTaskType" :items="menuTaskTypes"/>
        </div>
      </div>
      <v-container class="d-flex flex-column">
        <v-text-field
          variant="solo"
          label="Название задачи"
          v-on:keyup="e => taskNameKeyUpEventHandler(e)"
        />
        <v-text-field
          variant="solo" v-on:keyup="e => urlKeyUpEventHandler(e)"
          placeholder="https://localhost:3000/users/{user}?todo&part"
          hint="Используйте {param} для установки параметров в пути url"
        />
        <v-card v-if="urlParts.length !== 0" class="mb-5">
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
        <v-select variant="solo" label="Метод запросов" v-model="selectedHttpRequestMethod" :items="httpRequestMethods"/>
        <v-textarea v-model="postMethodBody" variant="solo" label="Тело запроса" v-if="selectedHttpRequestMethod === 'POST'"></v-textarea>
        <v-card v-if="headers.length !== 0" :flat="true">
          <v-card-title class="mb-3">Заголовки запроса</v-card-title>
          <v-list class="ma-0 pa-0">
            <v-list-item class="ma-0 pa-0" v-for="(header, index) in headers" :key="index">
              <v-row class="ma-0 pa-0 d-flex align-center">
                <v-col class="pa-1 ma-0">
                  <v-text-field hide-details label="Название" :readonly="true" persistent-placeholder variant="solo">{{header.name}}</v-text-field>
                </v-col>
                <v-col class="pa-1 ma-0">
                  <v-text-field hide-details label="Значение" :readonly="true" persistent-placeholder variant="solo">{{header.value}}</v-text-field>
                </v-col>
                <v-col cols="2" class="d-flex justify-center">
                  <v-btn @click="removeHeader(index)" color="red">
                    <v-icon>mdi-close</v-icon>
                  </v-btn>
                  <v-btn class="ml-4" @click="showAddHeaderDialog = true" color="primary">
                    <v-icon>mdi-plus</v-icon>
                  </v-btn>
                </v-col>
              </v-row>
            </v-list-item>
          </v-list>
        </v-card>
        <v-btn width="500px" v-if="headers.length === 0" @click="showAddHeaderDialog = true" color="primary">Добавить заголовок запроса</v-btn>
        <v-card v-if="tags.length !== 0 && selectedMenuTaskType === menuTaskTypes[2]" :flat="true" class="mt-4">
          <v-card-title>Теги которые необходимо спарсить</v-card-title>
          <v-list class="ma-0 pa-0">
            <v-list-item class="ma-0 pa-0" v-for="(tag, index) in tags" :key="index">
              <v-row class="ma-0 pa-0 d-flex align-center">
                <v-col>
                  <v-text-field hide-details label="Описание тега" :readonly="true" persistent-placeholder variant="solo">
                    {{`<${tag.name} * ${tag.attributes.map(x => `${x.name}="${x.value}"`).join(' ')} * >*<\/${tag.name}>`}}
                  </v-text-field>
                </v-col>
                <v-col cols="2" class="d-flex justify-center">
                  <v-btn @click="removeTag(index)" color="red">
                    <v-icon>mdi-close</v-icon>
                  </v-btn>
                  <v-btn class="ml-4" @click="showAddTagDialog = true" color="primary">
                    <v-icon>mdi-plus</v-icon>
                  </v-btn>
                </v-col>
              </v-row>
            </v-list-item>
          </v-list>
        </v-card>
        <v-btn width="500px" v-if="tags.length === 0 && selectedMenuTaskType === menuTaskTypes[2]" @click="showAddTagDialog = true" class="mt-4" color="primary">Добавить тег который необходимо спарсить</v-btn>
      </v-container>
      <v-spacer/>
      <v-card-actions class="pa-0 ma-0">
        <v-btn @click="createParserTask" :disabled="loadingCreateTask" :loading="loadingCreateTask" :block="true" color="primary" variant="flat">Создать</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

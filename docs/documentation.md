## Задача (Task)
```json
{
  "url": "https://en.wikipedia.org/wiki/Transhumanism/page/{page}/attribute",
  "type" 1, // 0 - api, 1 - website_text, 2 - website_tags,  3 - selenium
  "name": "Спарсить погоду",
  "websiteTagsOptions": {
    "tags": [
      {
        "findOptions": {
          "name": "p",
          "attributes": [
            {
              "name": "class",
              "value": "dsaodkas"
            }
          ]
        }
      }
    ]
  },
  "apiOptions": {
    "requestMethod": "POST", // GET POST PUT DELETE
    "postMethodOptions": {
      "requestBody": "{ 'id': 23 }"
    },
    "queries": [
      {
        "name": "page",
        "valueOptions": {
          "range": {
            "start": 1,
            "end": 100
          },
          "list": [ 23, 25, 30],
          "value": 5
        }
      }
    ],
    "path": [
      {
        "name": "page",
        "valueOptions": {
          "range": {
            "start": 1,
            "end": 100
          },
          "list": [ 23, 25, 30],
          "value": 5
        }
      }
    ],
    "headers": [
      {
        "name": "API_TOKEN",
        "value": "kasdksajdklj23lkjasdlks"
      }
    ]
  }
}
```

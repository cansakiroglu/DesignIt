import requests
import json

# set the API endpoint and your API key
endpoint = "https://api.sketchfab.com/v3/search"
api_key = "0d0c5741ed93477986ae00986540961b"

# set the parameters for the search query
params = {
    "q": "chair",
    "sort_by": "relevance",
    "type": "models",
    "features": "downloadable"
}

# send the GET request to the API endpoint
response = requests.get(endpoint, params=params, headers={"Authorization": "Token " + api_key})

# check the response status code
if response.status_code == 200:
    # process the JSON data returned by the API

    print(response.json())
    #next step
    data = response.json()
    #loop through the data and download the models
    with open('response.json', 'w') as f:
        f.write(json.dumps(response.json(), indent=4))
else:
    # print the error message
    print(response.json()["error"])

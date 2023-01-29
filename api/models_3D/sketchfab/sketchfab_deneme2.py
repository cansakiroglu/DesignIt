import requests
import json

# Replace YOUR_API_TOKEN with your actual API token
api_token = '0d0c5741ed93477986ae00986540961b'
headers = {'Authorization': 'Token ' + api_token}

# Replace MODEL_ID with the ID of the model you want to access
# Replace MODEL_ID with the ID of the model you want to download
model_id = "5aa086a9fea6468fae96b922bbd7a81b"


# Replace FILE_FORMAT with the desired file format (e.g. "glb", "obj", "fbx")
file_format = "gltf"

# Make the GET request to the download endpoint
response = requests.get(f"https://api.sketchfab.com/v3/models/{model_id}/download", headers=headers)

print(response.status_code)
with open('download_response.json', 'w') as f:
    f.write(json.dumps(response.json(), indent=4))

download_url=response.json()[file_format]['url']


response = requests.get(download_url)


with open('response.zip', 'wb') as f:
    f.write(response.content)

#unzip the file
import zipfile
with zipfile.ZipFile('response.zip', 'r') as zip_ref:
    zip_ref.extractall('response')

# # Save the file to disk


# with open('response.{file_format}'.format(file_format=file_format), 'wb') as f:
#     f.write(response.content)




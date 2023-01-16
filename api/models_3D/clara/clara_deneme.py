import requests
import json

url="https://clara.io/api/scenes?page=1&perPage=100&type=library&query=car&public=true"

api_token = "4cb3d51c-8054-47f0-b9c4-b1a44fc65e19"
username='bil496bbmm'

response = requests.get(url)

# with open('car.json', 'w') as f:
#     f.write(json.dumps(response.json(), indent=4))

scene_id = response.json()['models'][0]['_id']
print("scene_id: ",scene_id)



car_url='https://clara.io/api/scenes/{uuid}/export/{extension}?zip={doZip}&centerScene={doCenter}'.format(uuid=scene_id,extension='obj',doZip='false',doCenter='true')
header={'username':username,'apiToken':api_token}


response = requests.get(car_url,auth=(username, api_token))

url = "https://clara.io/view/bc5fa46d-3e39-4fad-a4ea-6f93712ed957"
response = requests.get(url)

print(response.text)
with open('response.obj', 'wb') as f:
    f.write(response.text)

#son durum: 401 no authorization, registration yapmam gerekiyor
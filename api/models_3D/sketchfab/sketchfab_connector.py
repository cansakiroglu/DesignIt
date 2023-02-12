import requests
import json
class SketchFabConnector:
    def __init__(self, token):
        self.token = token
        self.api_url = 'https://api.sketchfab.com/v3/'
        self.headers = {'Authorization': 'Token ' + self.token}
    
    def search(self, query, sort_by='relevance', type='models', features='downloadable'):
        params = {
            "q": query,
            "sort_by": sort_by,
            "type": type,
            "features": features
        }
        response = requests.get(self.api_url + 'search', params=params, headers=self.headers)
        if response.status_code == 200:
            return response.json()
        else:
            print(response.json()["error"])
            return None
    
    #TODO: search_downloadable method (call search method and filter the results)

    def parse_search_result(self,search_result,num_models=3):
        return [(item['uid'],item['name']) for item in search_result['results'] if item['isDownloadable']==True][:num_models]

    
    def download(self, model_id, file_format='gltf'):
        response = requests.get(self.api_url + 'models/' + model_id + '/download', headers=self.headers)
        if response.status_code == 200:
            download_url = response.json()[file_format]['url']
            response = requests.get(download_url)
            return response.content
        else:
            print(response.json())
            return None
    
    #TODO: unzip method
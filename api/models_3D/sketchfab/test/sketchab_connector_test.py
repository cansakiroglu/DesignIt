import sys
import os
#append parent directory to path
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
from sketchfab_connector import SketchFabConnector
import argparse
import json



def test_search(api_token,query):
    connector = SketchFabConnector(api_token)
    result = connector.search(query)  
    return result


def test_download(api_token,search_result,num_models=3):
    connector = SketchFabConnector(api_token)
    parse_result = connector.parse_search_result(search_result,num_models)
    model_ids = [item[0] for item in parse_result]
    model_names = [item[1] for item in parse_result]
    results=[]
    for model_id in model_ids:
        result = connector.download(model_id)
        results.append(result)
    
    
    return results,model_names


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Test Sketchfab Connector')
    parser.add_argument('--token','-t', type=str, required=True, help='API Token for Sketchfab')
    parser.add_argument('--querylist','-q', type=str, required=True, help='Query list for Sketchfab')
    parser.add_argument('--outfolder','-o', type=str, required=True, help='Output folder for Sketchfab')
    parser.add_argument('--nummodels','-n', type=int, required=False, default=3, help='Number of models to download for each query')
    parser.add_argument('--fileformat','-f', type=str, required=False, default='gltf', help='File format for downloaded models')
    parser.add_argument('--downloadfolder','-d', type=str, required=False, default='downloaded_models', help='Folder for downloaded models')
    
    args = parser.parse_args()
    fileformat = 'zip' if args.fileformat=='gltf' else args.fileformat
    with open(args.querylist) as f:
        queries = f.readlines()
    queries = [x.strip() for x in queries]
    for count,query in enumerate(queries):
        with open(os.path.join(args.outfolder,'search_result_{}.json'.format(count)), 'w') as f:
            result=test_search(args.token,query)
            f.write(json.dumps(result, indent=4))

    for search_result in os.listdir(args.outfolder):
        with open(os.path.join(args.outfolder,search_result)) as f:
            result_json = json.load(f)
        result=test_download(args.token,result_json,args.nummodels)
        
        for model,modelname in zip(result[0],result[1]):
    
            with open(os.path.join(args.downloadfolder,'{}.{}'.format(modelname,fileformat)), 'wb') as f:
                f.write(model)




        
    
 
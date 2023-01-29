"""
Sketchfab supports the Oauth2 protocol for authentication and authorization
### Introduction
- Please refer to the manual for missing/relevant information : https://tools.ietf.org/html/draft-ietf-oauth-v2-31
- This code sample does not work such as it is, it is a template that roughly shows how the token exchange works.
- This code sample is written in python but the same logic applies for every other language
### Requirements
To begin, obtain Oauth2 credentials from support@sketchfab.com.
You must provide us a redirect uri to which we can redirect your calls
### Implementation
The protocol works as follow:
1. You ask for an authorization code from the Sketchfab server with a supplied `redirect_uri`
2. Sketchfab asks permission to your user
3. A successful authorization will pass the client the authorization code in the URL via the supplied `redirect_uri`
4. You exchange this authorization code with an access token from the Sketchfab server
5. You use the access token to authenticate and authorize your user
"""

import requests

CLIENT_ID = "ykODUAxm74y4GySBFtGA83ZR4zkpbZtPyJP3Cjtd"
CLIENT_SECRET = "LI5TVd7EWInsqQFn96lVs4O3mP6iC19FkqBtGmYr2cZVDpufu5RBMgppanpj2uFP6wbGqmiUlIMEe6KHruYXsT867T5VCfXGMo6roLf4nSj1nVfj9YDHk5RsdSm4cQIZ"

REDIRECT_URI = 'localhost:8000'
AUTHORIZE_URL = "https://sketchfab.com/oauth2/authorize/"
ACCESS_TOKEN_URL = "https://sketchfab.com/oauth2/token/"

# 1. Ask for an authorization code


code_response= requests.get('{}?response_type=code&client_id={}&redirect_uri={}'.format(AUTHORIZE_URL, CLIENT_ID, REDIRECT_URI))

# 2. The user logs in, accepts your client authentication request

print(code_response)

# 3. Sketchfab redirects to your provided `redirect_uri` with the authorization code in the URL
# Ex : https://website.com/oauth2_redirect?code=123456789

# 4. Grab that code and exchange it for an `access_token`
access_token_response=requests.post(
    ACCESS_TOKEN_URL,
    data={
        'grant_type': 'authorization_code',
        'code': '123456789',
        'client_id': CLIENT_ID,
        'client_secret': CLIENT_SECRET,
        'redirect_uri': REDIRECT_URI
    }
)

print(access_token_response.json())

# The response body of this request contains the `access_token` necessary to authenticate your requests
# Ex : {"access_token": "1234", "token_type": "Bearer", "expires_in": 36000, "refresh_token": "5678", "scope": "read write"}
# - expires_in => seconds to live for this `access_token`
# - refresh_token => A token used to fetch a new `access_token` (See below)


# Now you're all set, the following request shows how to use your `access_token` in your requests
# If your access token is recognized, this will return information regarding the current user
response_deneme=requests.get('https://sketchfab.com/v2/users/me', headers={'Authorization': 'Bearer YOUR_ACCESS_TOKEN'})

print(response_deneme.json())

# Extra:

# Before your access token expires, you can refresh it with the `refresh_token`. If it has expired,
# you will have to re-do the auhorization workflow
requests.post(
    ACCESS_TOKEN_URL,
    data={
        'grant_type': 'refresh_token',
        'client_id': CLIENT_ID,
        'client_secret': CLIENT_SECRET,
        'refresh_token': 'YOUR_REFRESH_TOKEN'
    }
)

# The response body of this request is exactly the same as the one to get an access_token
# Ex : {"access_token": "1234", "token_type": "Bearer", "expires_in": 36000, "refresh_token": "5678", "scope": "read write"}
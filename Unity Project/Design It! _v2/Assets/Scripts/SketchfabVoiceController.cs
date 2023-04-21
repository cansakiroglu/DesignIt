/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.WitAi;
using Meta.WitAi.Json;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class SketchfabVoiceController : MonoBehaviour
{
    [SerializeField] public Oculus.Voice.AppVoiceExperience appVoiceExperience;

    [SerializeField] private RaySpawn raySpawn;

    [SerializeField] private GameObject overlayCanvas;
    [SerializeField] private TMPro.TMP_Text text;

    // Whether voice is activated
    public bool IsActive => _active;
    private bool _active = false;
    private bool is_this_task = false;

    // Add delegates
    private void OnEnable()
    {
        appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener(OnRequestStarted);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnRequestFullTranscript);
        appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnRequestResponse);
        appVoiceExperience.VoiceEvents.OnError.AddListener(OnRequestError);
    }
    // Remove delegates
    private void OnDisable()
    {
        appVoiceExperience.VoiceEvents.OnRequestCreated.RemoveListener(OnRequestStarted);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnRequestTranscript);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnRequestFullTranscript);
        appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnRequestResponse);
        appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnRequestError);
    }

    // Request began
    private void OnRequestStarted(WitRequest r)
    {
        
    }
    // Request transcript
    private void OnRequestTranscript(string transcript)
    {
        if(is_this_task){
            text.SetText(transcript);
        }
    }
    // Request transcript full
    private void OnRequestFullTranscript(string transcript)
    {
        if(is_this_task){
            text.SetText(transcript);
            RunNLPModel(transcript);
            StartCoroutine(HideOverlay(3));
            is_this_task = false;
        }
    }

    IEnumerator HideOverlay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        overlayCanvas.SetActive(false);
    }

    // Request response
    private void OnRequestResponse(WitResponseNode response)
    {
        SetActivation(false);
    }
    // Request error
    private void OnRequestError(string error, string message)
    {
        SetActivation(false);
    }
    // Deactivate
    private void OnRequestComplete()
    {
        SetActivation(false);
    }

    // Set activation
    public void SetActivation(bool toActivated)
    {
        if (_active != toActivated)
        {
            _active = toActivated;
            if (_active)
            {
                appVoiceExperience.Activate();
                overlayCanvas.SetActive(true);
                text.text = "Listening";
                is_this_task = true;
            }
            else
            {
                appVoiceExperience.Deactivate();
            }
        }
    }

    // NLP PART
    static readonly HttpClient client = new HttpClient();
    private async void RunNLPModel(string transcript){
        var modelId = "basakdemirok/my_furniture_ner_model";
        var apiToken = "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";
        var data = await QueryAsync(transcript, modelId, apiToken);
        if (!data.Contains("currently loading") && data.Contains("word")){
            // Find the index of the first occurrence of "word"
            int wordIndex = data.IndexOf("\"word\"");

            // Find the index of the next double quote after "word"
            int startQuote = data.IndexOf("\"", wordIndex + 6);

            // Find the index of the next double quote after the start quote
            int endQuote = data.IndexOf("\"", startQuote + 1);

            print(wordIndex);
            print(startQuote);
            print(endQuote);
            // Get the substring between the start and end quotes
            string word = data.Substring(startQuote + 1, endQuote - startQuote - 1);

            text.SetText(word);
            raySpawn.OpenMenu(word);
        }
        else if (data.Contains("currently loading")){
            text.SetText("Model loading. Wait 20sec.");
        } else{
            text.SetText("Failed to find specified object.");
        }
    }

    private async Task<string> QueryAsync(string payload, string modelId, string apiToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        string API_URL = $"https://api-inference.huggingface.co/models/{modelId}";
        var response = await client.PostAsync(API_URL, new StringContent(payload, System.Text.Encoding.UTF8, "application/json"));
        string responseBody = await response.Content.ReadAsStringAsync();
        print(responseBody);
        return responseBody;
    }
}

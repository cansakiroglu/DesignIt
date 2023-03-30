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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Oculus.Voice
{
    public class VoiceController : MonoBehaviour
    {
        [Header("Default States"), Multiline]
        [SerializeField] private string freshStateText = "Try pressing the Activate button and saying \"I want a table lamp.\"";

        [Header("Speech Recognition - UI")]
        [SerializeField] private Text SRTextArea;
        [SerializeField] private bool SRShowJson;

        [Header("NLP - UI")]
        [SerializeField] private Text NLPTextArea;
        [SerializeField] private bool NLPShowJson;

        [Header("Voice")]
        [SerializeField] private AppVoiceExperience appVoiceExperience;

        // Whether voice is activated
        public bool IsActive => _active;
        private bool _active = false;

        // Add delegates
        private void OnEnable()
        {
            SRTextArea.text = freshStateText;
            appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener(OnRequestStarted);
            appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnRequestTranscript);
            appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnRequestFullTranscript);
            appVoiceExperience.VoiceEvents.OnStartListening.AddListener(OnListenStart);
            appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(OnListenStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.AddListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.AddListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnRequestResponse);
            appVoiceExperience.VoiceEvents.OnError.AddListener(OnRequestError);
        }
        // Remove delegates
        private void OnDisable()
        {
            appVoiceExperience.VoiceEvents.OnRequestCreated.RemoveListener(OnRequestStarted);
            appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnRequestTranscript);
            appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnRequestFullTranscript);
            appVoiceExperience.VoiceEvents.OnStartListening.RemoveListener(OnListenStart);
            appVoiceExperience.VoiceEvents.OnStoppedListening.RemoveListener(OnListenStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.RemoveListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.RemoveListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnRequestResponse);
            appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnRequestError);
        }

        // Request began
        private void OnRequestStarted(WitRequest r)
        {
            // Store json on completion
            if (NLPShowJson) r.onRawResponse = (response) => SRTextArea.text = response;
            // Begin
            _active = true;
        }
        // Request transcript
        private void OnRequestTranscript(string transcript)
        {
            SRTextArea.text = transcript;
        }
        // Request transcript full
        private void OnRequestFullTranscript(string transcript)
        {
            Debug.Log("Debugging Full Transcript : " + transcript);
            RunNLPModel(transcript);
        }
        // Listen start
        private void OnListenStart()
        {
            SRTextArea.text = "Listening...";
        }
        // Listen stop
        private void OnListenStop()
        {
            SRTextArea.text = "Processing...";
        }
        // Listen stop
        private void OnListenForcedStop()
        {
            if (!NLPShowJson)
            {
                SRTextArea.text = freshStateText;
            }
            OnRequestComplete();
        }
        // Request response
        private void OnRequestResponse(WitResponseNode response)
        {
            if (!NLPShowJson)
            {
                if (!string.IsNullOrEmpty(response["text"]))
                {
                    SRTextArea.text = "I heard: " + response["text"];
                }
                else
                {
                    SRTextArea.text = freshStateText;
                }
            }
            OnRequestComplete();
        }
        // Request error
        private void OnRequestError(string error, string message)
        {
            if (!NLPShowJson)
            {
                SRTextArea.text = $"<color=\"red\">Error: {error}\n\n{message}</color>";
            }
            OnRequestComplete();
        }
        // Deactivate
        private void OnRequestComplete()
        {
            _active = false;
        }

        // Toggle activation
        public void ToggleActivation()
        {
            SetActivation(!_active);
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


            if (!NLPShowJson)
            {
                NLPTextArea.text = data;
            } else { // same for now
                NLPTextArea.text = data;
            }
        }

        private async Task<string> QueryAsync(string payload, string modelId, string apiToken)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            string API_URL = $"https://api-inference.huggingface.co/models/{modelId}";
            var response = await client.PostAsync(API_URL, new StringContent(payload, System.Text.Encoding.UTF8, "application/json"));
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}

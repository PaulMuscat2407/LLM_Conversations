using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WebAPICall : MonoBehaviour
{
    private string API_TOKEN = "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";
    private string User_Input, model, character, location,generated_text;
    private string[] generatedWords;
    private int currentWordIndex = 0;
    [SerializeField]
    private CharacterLocation selectedCharacterLocation;
    [SerializeField]
    private Image Character_Sprite,Background_Sprite;


    [SerializeField]
    private GameObject User_Input_Area, User_Input_Chat, Submit_Button, Character_Response_Panel,Scrollbar;
    private bool hasResponded,isBlinking;

    void Start()
    {
        model = "HuggingFaceH4/zephyr-7b-beta";
        character = selectedCharacterLocation.character.ToString().Replace('_',' ');
        location = selectedCharacterLocation.location.ToString().Replace('_',' ');;

        Character_Sprite.sprite  = LoadCharacterSprite(selectedCharacterLocation.character);
        Background_Sprite.sprite = LoadLocationBackground(selectedCharacterLocation.location);

        Character_Response_Panel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = character;
        Character_Response_Panel.SetActive(false);
    }

    void Update()
    {
        if (hasResponded)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopBlinking();

                Character_Response_Panel.SetActive(false);
                Character_Response_Panel.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().enabled = false;
                Character_Response_Panel.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text = "";
                User_Input_Area.SetActive(true);
                User_Input_Chat.GetComponent<TMP_InputField>().text = "";
                hasResponded = false;
            }
        }
    }

    void ProcessResponse(string jsonResponse)
    {
        List<GeneratedTextResponse> responseList = JsonConvert.DeserializeObject<List<GeneratedTextResponse>>(jsonResponse);
        string generatedText = responseList[0].generated_text;
        generatedWords = generatedText.Split(' ');
        Scrollbar.GetComponent<Scrollbar>().value = 0;

        StartCoroutine(RevealText());

        // Process JSON response as needed
        Debug.Log("Processed Response: " + generatedText);
        hasResponded = true;
    }

    private Sprite LoadCharacterSprite(CharacterType character)
    {
        switch (character)
        {
            case CharacterType.Elijah_Thorne_Inventor:
                return Resources.Load<Sprite>("Images/Characters/Elijah_Thorne");
            case CharacterType.Inspector_Rupert_Blackwell:
                return Resources.Load<Sprite>("Images/Characters/Inspector_Blackwell");
            case CharacterType.Lady_Constance_Fairchild_Spiritualist:
                return Resources.Load<Sprite>("Images/Characters/Lady_Constance");
            default:
                return null;
        }
    }

    private Sprite LoadLocationBackground(LocationType location)
    {
        switch (location)
        {
            case LocationType.Victorian_London_Street_Market:
                return Resources.Load<Sprite>("Images/Scenes/Victorian_Street_Market");
            case LocationType.Victorian_Steampunk_Workshop:
                return Resources.Load<Sprite>("Images/Scenes/Victorian_Steampunk_Workshop");
            case LocationType.Victorian_Psychic_Medium_Caravan:
                return Resources.Load<Sprite>("Images/Scenes/Victorian_Psychic_Medium_Caravan");
            default:
                return null;
        }
    }

    public void ReturnToMainMenu(){
        SceneManager.LoadScene("Menu Scene");
    }
    
    public void ValidateInput()
    {
        if (User_Input_Chat.GetComponent<TMP_InputField>().text == "")
            Submit_Button.GetComponent<Button>().interactable = false;
        else
            Submit_Button.GetComponent<Button>().interactable = true;
    }
    public void MakeAPICall()
    {
        StartCoroutine(SendRequestToHuggingFace(model, character, location));
        User_Input_Area.SetActive(false);

        Character_Response_Panel.SetActive(true);
    }
    void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkCoroutine());
        }
    }
    public void StopBlinking()
    {
        // Stop the blinking coroutine
        if (isBlinking)
        {
            isBlinking = false;
            StopCoroutine(BlinkCoroutine());
        }
    }

    IEnumerator SendRequestToHuggingFace(string model, string character, string location)
    {
        User_Input = User_Input_Chat.GetComponent<TMP_InputField>().text.ToString();
        string inputText = $"<|system|>The current location is {location},Respond as {character} who always responds in the first-person.</s>\n<|user|>{User_Input}</s><|assistant|>";
        string url = $"https://api-inference.huggingface.co/models/{model}";

        // Prepare the JSON data
        var data = new
        {
            inputs = inputText,
            parameters = new
            {
                return_full_text = false,
                max_new_tokens = 500,
                do_sample = true,
                temperature = 0.7,
                top_k = 50,
                top_p = 0.95
            }
        };

        string jsonData = JsonConvert.SerializeObject(data);

        // Create a new UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {API_TOKEN}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                ProcessResponse(request.downloadHandler.text);
            }
        }
    }

    IEnumerator RevealText()
    {
        while (currentWordIndex < generatedWords.Length)
        {
            // Append the next word to the text
            Character_Response_Panel.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text += generatedWords[currentWordIndex] + " ";
            currentWordIndex++;

            // Wait for a short duration
            yield return new WaitForSeconds(0.03f);
        }

        // Reset word index for future use
        currentWordIndex = 0;
        Character_Response_Panel.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().enabled = true;
        StartBlinking();
    }

    IEnumerator BlinkCoroutine()
    {
        float currentAlpha = 1f;
        float t = 0f;

        while (true)
        {
            // Fade out
            while (currentAlpha > 0.2f)
            {
                currentAlpha = Mathf.Lerp(1f, 0.2f, t);
                Character_Response_Panel.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().alpha = currentAlpha;
                t += Time.deltaTime / 2f;
                yield return null;
            }

            // Fade in
            while (currentAlpha < 1f)
            {
                currentAlpha = Mathf.Lerp(0.2f, 1f, t);
                Character_Response_Panel.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().alpha = currentAlpha;
                t += Time.deltaTime / 2f;
                yield return null;
            }

            t = 0f;
        }
    }
}

internal class GeneratedTextResponse
{
    public string generated_text;
}
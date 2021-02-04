using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.Android;
using System;

public class Main : MonoBehaviour
{
  public static Main Instance {get;set;}

  public GameObject redText;
  private string baseURL = "http://api.openweathermap.org/data/2.5/weather?q=Paris&appid=ae9df496280ed949ebf5dd26bff1da80";
  public GameObject Sun;
  
  private string testUrl = "";
  DateTime currentTime = System.DateTime.Now;
  private string actualWeather;
  JSONNode infoMeteo;


  [Header("Meteo objects")]

  public GameObject clouds;
  public GameObject rainObject;

  [Header("GPS ")]
  string longitude;
  string latitude;

  [Header("Textes UI ")]
  public Text[] texteUI;


  private void Awake()
  {
    checkHour();
  }
  void Start()
    {
    Instance = this;
    DontDestroyOnLoad(gameObject);
    if (Permission.HasUserAuthorizedPermission(Permission.FineLocation)) {
      // The user authorized use of the localisation.
      StartCoroutine(StartLocation());

    }
    else {
      // We do not have permission to use the microphone.
      // Ask for permission or proceed without the functionality enabled.
      Permission.RequestUserPermission(Permission.FineLocation);
    }
    StartCoroutine(GetBaseIndex());

    testUrl = baseURL + "/";
   
    }


  IEnumerator GetBaseIndex ()
  {
    UnityWebRequest infoRequest = UnityWebRequest.Get(baseURL);
    yield return infoRequest.SendWebRequest();

    if (infoRequest.isNetworkError || infoRequest.isHttpError) {
      Debug.LogError(infoRequest.error);
      yield break;
    }

    infoMeteo = JSON.Parse(infoRequest.downloadHandler.text);
    redText.GetComponent<Text>().text = infoMeteo["weather"][0]["main"];
    actualWeather = infoMeteo["weather"][0]["main"];
    
    // I get the temperature in Kevin, I convert it in celsius and I display it o the interface
    string stringTempKelvin = infoMeteo["main"]["temp"];

    int index = stringTempKelvin.IndexOf(".");
    if (index >= 0) {
      stringTempKelvin = stringTempKelvin.Substring(0, index);

    }
    int tempKelvin = int.Parse(stringTempKelvin);
    
    redText.GetComponent<Text>().text = stringTempKelvin;

    float tempCelsius = tempKelvin - 273.15f;
    texteUI[0].text = Input.location.isEnabledByUser.ToString();
    texteUI[1].text = infoMeteo["coord"]["lat"];
    texteUI[2].text = infoMeteo["coord"]["lon"];
    texteUI[3].text = tempCelsius.ToString()+" C°";
    texteUI[4].text = infoMeteo["weather"][0]["main"];
    manageWeather();

  }

  // I instantiate gameObject in terms of the value of actualWeather 
  void manageWeather()
  {
    clouds.SetActive(true);
    rainObject.SetActive(false);
    Debug.Log("Actual weather : " + actualWeather);
     switch (actualWeather) {
      case "Clear":
        clouds.SetActive(false);

        break;
      case "Clouds":
        break;
      case "Rain":
        rainObject.SetActive(true);

        break;
      default:
        break;
    }
  }

  // Hasn't been implemented yet. It consists in letting the user select a city to check its weather 
  void changeCity(string cityName)
  {
    baseURL = "http://api.openweathermap.org/data/2.5/weather?q="+cityName+"&appid=ae9df496280ed949ebf5dd26bff1da80";
    // Change sun position too 

    StartCoroutine(GetBaseIndex());

  }

  // Hours  
  void checkHour()
  {
    if (currentTime.Hour == 12) {
      Sun.transform.localPosition = new Vector3(0.006001374f, 1.674049f, -0.21361f);
      Sun.transform.rotation =Quaternion.Euler(-4.801f,0,0);
    }

    else if (currentTime.Hour>12 && currentTime.Hour <= 18) {
      Sun.transform.localPosition = new Vector3(0.0060012f, 1.496878f, 0.4282672f);
      Sun.transform.rotation = Quaternion.Euler(26.399f, 0, 0);
    }

    else if (currentTime.Hour >18 && currentTime.Hour <=23) {
      Sun.transform.localPosition = new Vector3(0.006001691f, -0.771477f, 0.0636064f);
      Sun.transform.rotation = Quaternion.Euler(162.599f, 0, 0);
    }

    else if (currentTime.Hour == 00 ) {
      Sun.transform.localPosition = new Vector3(0.006002243f, -0.7300884f, -0.6259816f);
      Sun.transform.rotation = Quaternion.Euler(-165f, 0, 0);
    }

    else if (currentTime.Hour >= 1 && currentTime.Hour <=5) {
      Sun.transform.localPosition = new Vector3(0.006002004f, -0.1636022f, -1.293185f);
      Sun.transform.rotation = Quaternion.Euler(-123.6f, 0, 0);
    }

    else {
      Sun.transform.localPosition = new Vector3(0.006002209f, 1.554713f, -0.7404106f);
      Sun.transform.rotation = Quaternion.Euler(-30f, 0, 0);
    }

  }

  // GEOLOCATION
  IEnumerator StartLocation()
  {
    Debug.Log("StartLocation");
    // First, check if user has location service enabled
    Debug.Log("Location  enabled ? : " + Input.location.isEnabledByUser);
    if (!Input.location.isEnabledByUser) {
      yield break;

    }

    // Start service before querying location
    Input.location.Start();

    // Wait until service initializes
    int maxWait = 120;
    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
      yield return new WaitForSeconds(1);
      maxWait--;
    }

    // Service didn't initialize 
    if (maxWait < 1) {
      Debug.Log("Timed out");
      //redText.GetComponent<Text>().text = "Timed out";
      yield break;
    }

    // Connection has failed
    if (Input.location.status == LocationServiceStatus.Failed) {
      Debug.Log("Unable to determine device location");
      yield break;
    }
    else {
      // Access granted and location value could be retrieved
      Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
      //redText.GetComponent<Text>().text = Input.location.lastData.latitude.ToString();

      //Here I get the coordinates
      longitude = Input.location.lastData.longitude.ToString();
      latitude = Input.location.lastData.latitude.ToString();
      baseURL= "api.openweathermap.org/data/2.5/weather?lat="+latitude+"&lon="+longitude+ "&appid=ae9df496280ed949ebf5dd26bff1da80";
    }
    // Stop service if there is no need to query location updates continuously
    Input.location.Stop();
    {
      Debug.Log("StopLocation");

    }
    StartCoroutine(GetBaseIndex());
  }

}

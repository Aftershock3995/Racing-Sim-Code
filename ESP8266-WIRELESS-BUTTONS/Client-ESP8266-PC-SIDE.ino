#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <Joystick.h>

const char* ssid = "wifi ssid";
const char* password = "wifi password";
const char* serverIP = "server-ip"; //(server ESP8266)

Joystick_ Joystick;

void setup() 
{
  Serial.begin(115200);

  // Connect to Wi-Fi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }
  Serial.println("Connected to WiFi");

  Joystick.begin();
}

void loop() 
{
 
  if (WiFi.status() == WL_CONNECTED) 
  {
    HTTPClient http;
    http.begin("http://" + String(serverIP));
    int httpCode = http.GET();
    if (httpCode == 200) 
    {
      String payload = http.getString();
      bool buttonState1 = payload[0] == '1';
      bool buttonState2 = payload[1] == '1';

      if (buttonState1) 
      {
        Joystick.setButton(1, 1); 
      } else 
      {
        Joystick.setButton(1, 0);  
      }

      if (buttonState2) 
      {
        Joystick.setButton(2, 1);  
      } else 
      {
        Joystick.setButton(2, 0); 
      }
    }
    http.end();

    delay(100);
  }
}

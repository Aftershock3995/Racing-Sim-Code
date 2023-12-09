#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ArduinoOTA.h>

const char* ssid = "wifi ssid";
const char* password = "password";

ESP8266WebServer server(80);
int buttonPin1 = D1;  
int buttonPin2 = D2; 
bool buttonState1 = LOW;
bool buttonState2 = LOW;

void handleRoot() 
{
  buttonState1 = digitalRead(buttonPin1);
  buttonState2 = digitalRead(buttonPin2);
  
  String response = String(buttonState1) + String(buttonState2);
  server.send(200, "text/plain", response);
}

void setup() 
{
  Serial.begin(115200);

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }
  Serial.println("Connected to WiFi");

  ArduinoOTA.begin();

  server.on("/", HTTP_GET, handleRoot);

  server.begin();
}

void loop() {
  ArduinoOTA.handle();

  server.handleClient();
}

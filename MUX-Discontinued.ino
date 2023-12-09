#include <Arduino.h>

const int muxS0 = 2;
const int muxS1 = 3;
const int muxS2 = 4;
const int muxS3 = 5;
const int muxSIG = 6;

// Enable pin for the reading MUX
const int enableReadingMUX = 7;

// Enable pin for the writing MUX
const int enableWritingMUX = 8;

void setup() {
  pinMode(muxS0, OUTPUT);
  pinMode(muxS1, OUTPUT);
  pinMode(muxS2, OUTPUT);
  pinMode(muxS3, OUTPUT);
  pinMode(muxSIG, INPUT);
  pinMode(enableReadingMUX, OUTPUT);
  pinMode(enableWritingMUX, OUTPUT);

  // Initially, enable the reading MUX and disable the writing MUX
  digitalWrite(enableReadingMUX, HIGH);
  digitalWrite(enableWritingMUX, LOW);

  digitalWrite(muxS0, LOW);
  digitalWrite(muxS1, LOW);
  digitalWrite(muxS2, LOW);
  digitalWrite(muxS3, LOW);

  Serial.begin(9600);
}

int readSensor(int sensorPin) {
  if (sensorPin < 0 || sensorPin > 15) {
    return -1;  // Invalid sensor pin
  }

  // Configure the reading MUX to select the appropriate sensor pin
  digitalWrite(muxS0, sensorPin & 0x01);
  digitalWrite(muxS1, (sensorPin >> 1) & 0x01);
  digitalWrite(muxS2, (sensorPin >> 2) & 0x01);
  digitalWrite(muxS3, (sensorPin >> 3) & 0x01);

  return digitalRead(muxSIG);
}

void writeData(int writePin, int data) {
  // Disable the reading MUX and enable the writing MUX
  digitalWrite(enableReadingMUX, LOW);
  digitalWrite(enableWritingMUX, HIGH);

  // Configure the writing MUX to select the appropriate destination pin
  digitalWrite(muxS0, writePin & 0x01);
  digitalWrite(muxS1, (writePin >> 1) & 0x01);
  digitalWrite(muxS2, (writePin >> 2) & 0x01);
  digitalWrite(muxS3, (writePin >> 3) & 0x01);

  // Write data to the selected destination pin
  digitalWrite(muxSIG, data);

  // Re-enable the reading MUX and disable the writing MUX
  digitalWrite(enableReadingMUX, HIGH);
  digitalWrite(enableWritingMUX, LOW);
}

void loop() {
  // Reading example
  int sensorValue = readSensor(14);
  Serial.print("Sensor 14 Value: ");
  Serial.println(sensorValue);

  // Writing example
  int writePin = 10;  // Replace with the actual destination pin to write data
  int data = HIGH;    // Replace with the actual data to write (HIGH or LOW)
  writeData(writePin, data);
  delay(1000);
}

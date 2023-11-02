// Define the control pins for the first CD74HC4067SM96 for compression (pedal end)
int S0_Pedal1 = 2;  // Replace with actual pin numbers
int S1_Pedal1 = 3;
int S2_Pedal1 = 4;
int S3_Pedal1 = 5;

// Define the control pins for the second CD74HC4067SM96 for compression (pedal end)
int S0_Pedal2 = 6;
int S1_Pedal2 = 7;
int S2_Pedal2 = 8;
int S3_Pedal2 = 9;

// Define the control pins for the second CD74HC4067SM96 for decompression (PCB Teensy end)
int S0_PCB2 = 10;
int S1_PCB2 = 11;
int S2_PCB2 = 12;
int S3_PCB2 = 13;

// Define the pins connected to the first CD74HC4067SM96 outputs on the pedal end
int sensorOutputPin1 = 14;
int sensorOutputPin2 = 15;

// Define the pins connected to the second CD74HC4067SM96 outputs on the pedal end
int sensorOutputPin3 = 16;
int sensorOutputPin4 = 17;

// Define the pins connected to the second CD74HC4067SM96 outputs on the PCB Teensy end
int sensorPin1 = 18;
int sensorPin2 = 19;
int sensorPin3 = 20;
int sensorPin4 = 21;
int sensorPin5 = 22;
int sensorPin6 = 23;
int sensorPin7 = 24;
int sensorPin8 = 25;
int sensorPin9 = 26;
int sensorPin10 = 27;
int sensorPin11 = 28;
int sensorPin12 = 29;
int sensorPin13 = 30;
int sensorPin14 = 31;
int sensorPin15 = 32;
int sensorPin16 = 33;

void setup() {
  // Set the control pins for the first CD74HC4067SM96 to their initial state
  pinMode(S0_Pedal1, OUTPUT);
  pinMode(S1_Pedal1, OUTPUT);
  pinMode(S2_Pedal1, OUTPUT);
  pinMode(S3_Pedal1, OUTPUT);
  
  // Set the control pins for the second CD74HC4067SM96 to their initial state
  pinMode(S0_Pedal2, OUTPUT);
  pinMode(S1_Pedal2, OUTPUT);
  pinMode(S2_Pedal2, OUTPUT);
  pinMode(S3_Pedal2, OUTPUT);
  
  pinMode(S0_PCB2, OUTPUT);
  pinMode(S1_PCB2, OUTPUT);
  pinMode(S2_PCB2, OUTPUT);
  pinMode(S3_PCB2, OUTPUT);
  
  // Initialize Serial communication for output (optional)
  Serial.begin(9600);
}

void loop() {
  // Compress (pedal end): Loop through each input channel (0 to 15) on the first CD74HC4067SM96
  for (int channel = 0; channel < 16; channel++) {
    // Set the control pins to select the current channel for compression
    digitalWrite(S0_Pedal1, channel & 1);
    digitalWrite(S1_Pedal1, (channel >> 1) & 1);
    digitalWrite(S2_Pedal1, (channel >> 2) & 1);
    digitalWrite(S3_Pedal1, (channel >> 3) & 1);
    
    // Read the sensor value from the sensorOutputPin1 on the first CD74HC4067SM96
    int sensorValue_Pedal1 = digitalRead(sensorOutputPin1);
    
    // Process the sensorValue_Pedal1 as needed
    if (sensorValue_Pedal1 == HIGH) {
      // Do something when the sensor is activated
      Serial.println("Sensor activated on channel " + String(channel));
    }
    
    // Add a delay if necessary
    delay(100);
  }
  
  // Compress (pedal end): Loop through each input channel (0 to 15) on the second CD74HC4067SM96
  for (int channel = 0; channel < 16; channel++) {
    // Set the control pins to select the current channel for compression on the second CD74HC4067SM96
    digitalWrite(S0_Pedal2, channel & 1);
    digitalWrite(S1_Pedal2, (channel >> 1) & 1);
    digitalWrite(S2_Pedal2, (channel >> 2) & 1);
    digitalWrite(S3_Pedal2, (channel >> 3) & 1);
    
    // Read the sensor value from the sensorOutputPin2 on the second CD74HC4067SM96
    int sensorValue_Pedal2 = digitalRead(sensorOutputPin2);
    
    // Process the sensorValue_Pedal2 as needed
    if (sensorValue_Pedal2 == HIGH) {
      // Do something when the sensor is activated
      Serial.println("Sensor activated on channel " + String(channel + 16));
    }
    
    // Add a delay if necessary
    delay(100);
  }

  // Decompress (PCB Teensy end): Loop through each output channel (0 to 15)
  for (int channel = 0; channel < 16; channel++) {
    // Set the control pins to select the current channel for decompression on the second CD74HC4067SM96
    digitalWrite(S0_PCB2, channel & 1);
    digitalWrite(S1_PCB2, (channel >> 1) & 1);
    digitalWrite(S2_PCB2, (channel >> 2) & 1);
    digitalWrite(S3_PCB2, (channel >> 3) & 1);
    
    // Read the sensor value from the selected channel on PCB Teensy end
    int sensorValue_PCB2 = digitalRead(getSensorPin(channel));
    
    // Process the sensorValue_PCB2 as needed
    if (sensorValue_PCB2 == HIGH) {
      // Do something when the sensor is activated
      Serial.println("Sensor activated on channel " + String(channel));
    }
    
    // Add a delay if necessary
    delay(100);
  }
}

// Helper function to return the appropriate sensorPin based on the channel
int getSensorPin(int channel) {
  switch (channel) {
    case 0:
      return sensorPin1;
    case 1:
      return sensorPin2;
    case 2:
      return sensorPin3;
    case 3:
      return sensorPin4;
    case 4:
      return sensorPin5;
    case 5:
      return sensorPin6;
    case 6:
      return sensorPin7;
    case 7:
      return sensorPin8;
    case 8:
      return sensorPin9;
    case 9:
      return sensorPin10;
    case 10:
      return sensorPin11;
    case 11:
      return sensorPin12;
    case 12:
      return sensorPin13;
    case 13:
      return sensorPin14;
    case 14:
      return sensorPin15;
    case 15:
      return sensorPin16;
    default:
      return -1; // Invalid channel
  }
}

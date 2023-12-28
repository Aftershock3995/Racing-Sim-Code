#include <AccelStepper.h>
#include <Encoder.h>

// pins for stepper motors
#define STEPPER_PIN_1_STEP 2
#define STEPPER_PIN_1_DIR 3
#define STEPPER_PIN_2_STEP 4
#define STEPPER_PIN_2_DIR 5
#define STEPPER_PIN_3_STEP 6
#define STEPPER_PIN_3_DIR 7

// pins for limit switches
#define LIMIT_SWITCH_1_FRONT 8
#define LIMIT_SWITCH_1_BACK 9
#define LIMIT_SWITCH_2_FRONT 10
#define LIMIT_SWITCH_2_BACK 11
#define LIMIT_SWITCH_3_FRONT 12
#define LIMIT_SWITCH_3_BACK 13

// pins for load cells and amplifiers
#define LOAD_CELL_PIN_1 A0
#define LOAD_CELL_PIN_2 A1
#define LOAD_CELL_PIN_3 A2

// analog input for adjustable spring constant
#define SPRING_CONSTANT_PIN 14

// pins for rotary encoders
#define ENCODER_1_A A4
#define ENCODER_1_B A5
#define ENCODER_2_A A6
#define ENCODER_2_B A7
#define ENCODER_3_A A8
#define ENCODER_3_B A9

// Create instances of AccelStepper for each stepper motor
AccelStepper stepper1(AccelStepper::DRIVER, STEPPER_PIN_1_STEP, STEPPER_PIN_1_DIR);
AccelStepper stepper2(AccelStepper::DRIVER, STEPPER_PIN_2_STEP, STEPPER_PIN_2_DIR);
AccelStepper stepper3(AccelStepper::DRIVER, STEPPER_PIN_3_STEP, STEPPER_PIN_3_DIR);

// Create instances of Encoder for each rotary encoder
Encoder encoder1(ENCODER_1_A, ENCODER_1_B);
Encoder encoder2(ENCODER_2_A, ENCODER_2_B);
Encoder encoder3(ENCODER_3_A, ENCODER_3_B);

void setup() 
{
  // Initialize Serial communication for debugging
  Serial.begin(9600);

  // Set up stepper motor properties
  stepper1.setMaxSpeed(1000);
  stepper2.setMaxSpeed(1000);
  stepper3.setMaxSpeed(1000);

  // Set up limit switches as inputs
  pinMode(LIMIT_SWITCH_1_FRONT, INPUT_PULLUP);
  pinMode(LIMIT_SWITCH_1_BACK, INPUT_PULLUP);
  pinMode(LIMIT_SWITCH_2_FRONT, INPUT_PULLUP);
  pinMode(LIMIT_SWITCH_2_BACK, INPUT_PULLUP);
  pinMode(LIMIT_SWITCH_3_FRONT, INPUT_PULLUP);
  pinMode(LIMIT_SWITCH_3_BACK, INPUT_PULLUP);

  // Set up load cells and amplifiers
  // Add your load cell initialization code here

  // Set up analog input for adjustable spring constant
  pinMode(SPRING_CONSTANT_PIN, INPUT);

  // Set up rotary encoder pins as inputs
  pinMode(ENCODER_1_A, INPUT);
  pinMode(ENCODER_1_B, INPUT);
  pinMode(ENCODER_2_A, INPUT);
  pinMode(ENCODER_2_B, INPUT);
  pinMode(ENCODER_3_A, INPUT);
  pinMode(ENCODER_3_B, INPUT);
}

void loop()
{
  // Perform homing sequence and update pedal position for each pedal
  homePedal(stepper1, LIMIT_SWITCH_1_FRONT, LIMIT_SWITCH_1_BACK);
  updatePedal(stepper1, encoder1);

  homePedal(stepper2, LIMIT_SWITCH_2_FRONT, LIMIT_SWITCH_2_BACK);
  updatePedal(stepper2, encoder2);

  homePedal(stepper3, LIMIT_SWITCH_3_FRONT, LIMIT_SWITCH_3_BACK);
  updatePedal(stepper3, encoder3);

  // Read analog input for adjustable spring constant
  int springConstant = analogRead(SPRING_CONSTANT_PIN);

  // Run the steppers
  stepper1.run();
  stepper2.run();
  stepper3.run();

  // Add your joystick control logic here for each pedal
  int joystickValue1 = map(encoder1.read(), 0, 1023, -512, 512);
  int joystickValue2 = map(encoder2.read(), 0, 1023, -512, 512);
  int joystickValue3 = map(encoder3.read(), 0, 1023, -512, 512);

  // Use joystick values as needed
  // For example, you can use them to control a joystick interface or send them to a computer

  // Add any additional logic or features as needed
}

void homePedal(AccelStepper& stepper, int frontLimitPin, int backLimitPin) 
{
  // Move to the front limit switch
  while (digitalRead(frontLimitPin) == HIGH) 
  {
    stepper.moveTo(stepper.currentPosition() + 1);
    stepper.run();
    delay(1);
  }

  // Set the position to 0
  stepper.setCurrentPosition(0);

  // Move to the back limit switch
  while (digitalRead(backLimitPin) == HIGH) 
  {
    stepper.moveTo(stepper.currentPosition() - 1);
    stepper.run();
    delay(1);
  }

  // Set the position to the max limit
  stepper.setCurrentPosition(stepper.currentPosition());
}

void updatePedal(AccelStepper& stepper, Encoder& encoder) 
{
  // Read rotary encoder value and calculate pedal position
  long encoderValue = encoder.read();
  int newPosition = calculateNewPosition(encoderValue, 0);  // Spring constant is set to 0 for joystick mode

  // Move the stepper to the new position
  stepper.moveTo(newPosition);
}

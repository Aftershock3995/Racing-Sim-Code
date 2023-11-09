**For the Reading Multiplexer (Pedal End):**

- **SIG (Z)**: Connect to Teensy's input pin where you want to read data.
- **GND**: Connect to Teensy's ground (GND).
- **Vcc**: Connect to Teensy's power supply (usually 5V for logic).
- **S0**: Connect to one of Teensy's digital pins to select the first channel.
- **S1**: Connect to another Teensy digital pin to select the second channel.
- **S2**: Connect to another Teensy digital pin to select the third channel.
- **S3**: Connect to another Teensy digital pin to select the fourth channel.

**For the Writing Multiplexer (Teensy End):**

- **Y0 to Y15**: Connect each of these outputs to the 16 pins where you want to send data or control.
- **GND**: Connect to Teensy's ground (GND).
- **Vcc**: Connect to Teensy's power supply (usually 5V for logic).
- **S0**: Connect to one of Teensy's digital pins to select the first destination pin.
- **S1**: Connect to another Teensy digital pin to select the second destination pin.
- **S2**: Connect to another Teensy digital pin to select the third destination pin.
- **S3**: Connect to another Teensy digital pin to select the fourth destination pin.
- **E (Enable Pin)**: Connect to a Teensy digital pin to enable or disable the writing multiplexer as needed.

*Note*: The provided code supports both reading and writing, but the pin setup currently supports reading only due to the 4-pin limit. Additional hardware, such as a bus, will be added later to handle the writing switching if needed.

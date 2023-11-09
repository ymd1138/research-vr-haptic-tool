/* Unityからデータを受信して、ソレノイドをオンオフするプログラム
* 0を受け取るとOFF、1を受け取るとON 
*/

char data;
int solenoid = 3;
int once = 1;

void setup() {
  Serial.begin(115200);
  pinMode(LED_BUILTIN, OUTPUT);  // 内蔵LED
  pinMode(solenoid, OUTPUT);

  // テストで光らせる
  delay(1000);
  digitalWrite(LED_BUILTIN, HIGH);
  delay(2000);
  digitalWrite(LED_BUILTIN, LOW);
  delay(1000);
}

void loop() {
  //Checks wether the code works properly on the Arduino
  if (once == 1) {
    digitalWrite(LED_BUILTIN, HIGH);
    delay(2000);
    digitalWrite(LED_BUILTIN, LOW);
    delay(1000);
    once = 0;
  }

  if (Serial.available() > 0) {  //受信データがあるか？
    data = Serial.read();        //1文字だけ読み込む
    if (data == 0x31) {          //文字コード0x31、つまり"1"を受信したらLEDを光らせる。
      digitalWrite(solenoid, HIGH);
      delay(200);
    } else if (data == 0x30) {  //文字コード0x30、つまり"0"を受信したらLEDを消す。
      digitalWrite(solenoid, LOW);
      delay(200);
    }
  }
}

// 0を受け取ると消灯し、1を受け取ると点灯するプログラム

#define LED_PIN 13 //LEDのピン番号。13は内蔵LED。
char data;

void setup() {
  Serial.begin(115200);
  pinMode(LED_PIN, OUTPUT);
}

void loop()
{
  if ( Serial.available()  > 0 ) {  //受信データがあるか？
    data = Serial.read();           //1文字だけ読み込む
    if (data == 0x31) {             //文字コード0x31、つまり"1"を受信したらLEDを光らせる。
      digitalWrite(LED_PIN, HIGH);
      delay(200);
    }
    else if (data == 0x30) {        //文字コード0x30、つまり"0"を受信したらLEDを消す。
      digitalWrite(LED_PIN, LOW);
      delay(200);
    }
  }
}

#include <EEPROM.h>

//----------------------------------------------------------------Main Value
const int EN = 5;
const int SW = 4;

int Count = 0, rev = 0;
unsigned long currentMillis = 0, previousMillis = 0;
int Amp = 0;
byte Speed = 0;
bool swState = 0;

bool Trig = 0;

//----------------------------------------------------------------Set Value

long int RevSet = 0, TimeSet = 0;
long int AmpStartSet = 0, AmpEndSet = 0;
String MotorSet;
int ErrorSet = 0;

//----------------------------------------------------------------Rx Value

char dataIn;
String waitProcess, dataAll;
int CutDataAll;

String strRev, strTime, strAmpStart, strAmpEnd, strMotorStatus, strError;
String OutRange1, OutRange2, OutRange3, OutRange4, OutRange5;

//----------------------------------------------------------------Tx Value
bool stState = 0;
bool ndState = 0;
bool rdState = 0;
bool th4_State = 0;
bool th5_State = 0;

String TxRev, TxTime;
String TxMotorStatus = "Stop";
String TxAmpStart, TxAmpEnd;
String TxError;
String TxAll;

String TxRealRev, TxRealTime, TxRealAmp, TxCheck;

const int OK = 22, NG = 23;
unsigned long currentMillis_OK = 0, previousMillis_OK = 0;
unsigned long currentMillis_NG = 0, previousMillis_NG = 0;

//----------------------------------------------------------------Amp Smooting (Low Pass Filter)

const int numReadings = 10;
const int inputPin = A0;

int readings[numReadings];
int readIndex = 0;
int total = 0;
int average = 0;

//----------------------------------------------------------------

void setup()
{
  Serial.begin(9600);
  Serial1.begin(9600);
  pinMode(SW, INPUT_PULLUP);
  pinMode(EN, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);

  EEPROM.get(0, RevSet);
  EEPROM.get(10, TimeSet);
  EEPROM.get(20, AmpStartSet);
  EEPROM.get(30, AmpEndSet);
  EEPROM.get(40, ErrorSet);

  attachInterrupt(digitalPinToInterrupt(2), REV, RISING);

  for (int thisReading = 0; thisReading < numReadings; thisReading++)
  {
    readings[thisReading] = 0;
  }

  Speed = 0;;
}

void loop()
{
  swState = digitalRead(SW);

  RealAmp();
  RxDataAll();

  if (CutDataAll > 0 )
  {
    if (strRev != "-")
    {
      RevSet = strRev.toInt();
    }
    if (strTime != "-")
    {
      TimeSet = strTime.toInt();
    }
    if (strAmpStart != "-")
    {
      AmpStartSet = strAmpStart.toInt();
    }
    if (strAmpEnd != "-")
    {
      AmpEndSet = strAmpEnd.toInt();
    }
    if (strMotorStatus != "-")
    {
      MotorSet = strMotorStatus;
    }
    if (strError != "-")
    {
      ErrorSet = strError.toInt();
    }

    TxRev = RevSet;
    TxTime = TimeSet;
    TxAmpStart = AmpStartSet;
    TxAmpEnd = AmpEndSet;
    TxMotorStatus = MotorSet;
    TxError = ErrorSet;

    EEPROM.put(0, RevSet);
    EEPROM.put(10, TimeSet);
    EEPROM.put(20, AmpStartSet);
    EEPROM.put(30, AmpEndSet);
    EEPROM.put(40, ErrorSet);

    TxAll = TxRev + "/" + TxTime + "/" + TxAmpStart + "/" + TxAmpEnd + "/" + TxMotorStatus + "/" + TxError + ";";
    Serial.print(TxAll);

    waitProcess = "";
    dataAll = "";
    CutDataAll = 0;
  }

  Check();
}

void RealAmp()
{
  total = total - readings[readIndex];
  readings[readIndex] = analogRead(inputPin);
  total = total + readings[readIndex];
  readIndex = readIndex + 1;

  if (readIndex >= numReadings)
  {
    readIndex = 0;
  }
  average = (total / numReadings) - 511;
  Amp = map(average, 0, 500, 0, 100000);
}

void REV()
{
  Count++;
  if (Count >= 10)
  {
    rev++;
    Count = 0;
  }
}

void Check()
{
  currentMillis = millis();
  currentMillis_OK = millis();
  currentMillis_NG = millis();

  if (MotorSet == "Run")
  {
    if (swState == 0 && Trig == 0)
    {
      stState = 0;
      ndState = 0;
      rdState = 0;
      Speed = 255;
      analogWrite(EN, Speed);
    }
    else if (swState == 1 && Trig == 1)
    {
      Trig = 0;
    }
    if (th4_State == 1 && currentMillis_OK - previousMillis_OK >= 2000)
    {
      digitalWrite(OK, 0);
      th4_State = 0;
    }
    if (th5_State == 1 && currentMillis_NG - previousMillis_NG >= 2000)
    {
      digitalWrite(NG, 0);
      th5_State = 0;
    }
    if (Amp < AmpStartSet)
    {
      previousMillis = currentMillis;
      rev = 0;  Count = 0;
      TxRealRev = rev;
      TxRealTime = "0";
      TxRealAmp = Amp;

      Serial1.print(TxAll);
      Serial1.print("\t");
      Serial1.print(TxRealRev);
      Serial1.print("\t");
      Serial1.print(TxRealTime);
      Serial1.print("\t");
      Serial1.println(TxRealAmp);
    }
    else if (currentMillis - previousMillis < TimeSet && (Amp >= AmpStartSet && Amp < AmpEndSet))
    {
      TxRealRev = rev;
      TxRealTime = currentMillis - previousMillis;
      TxRealAmp = Amp;

      Serial1.print(TxAll);
      Serial1.print("\t");
      Serial1.print(TxRealRev);
      Serial1.print("\t");
      Serial1.print(TxRealTime);
      Serial1.print("\t");
      Serial1.println(TxRealAmp);
    }
    else if ((currentMillis - previousMillis >= TimeSet ) || Amp >= AmpEndSet)
    {
      if (rev <= RevSet && ndState == 0 && Amp >= AmpEndSet)
      {
        TxCheck = "Hole Fit";
        ndState = 1;
      }
      if (rev >= RevSet && rev <= ( RevSet + ErrorSet ) && stState == 0 && Amp >= AmpEndSet)
      {
        TxCheck = "OK";
        th4_State = 1;
        previousMillis_OK = currentMillis_OK;
        digitalWrite(OK, 1);

        Speed = 0;
        analogWrite(EN, Speed);
        stState = 1;

        Trig = 1;
      }
      if (rev > ( RevSet + ErrorSet ) && rdState == 0 && ( Amp < AmpEndSet || Amp >= AmpEndSet))
      {
        TxCheck = "BlackHole";
        th5_State = 1;
        previousMillis_NG = currentMillis_NG;
        digitalWrite(NG, 1);

        Speed = 0;
        analogWrite(EN, Speed);
        rdState = 1;

        Trig = 1;
      }

      TxRealRev = rev;
      TxRealTime = currentMillis - previousMillis;
      TxRealAmp = Amp;

      Serial1.print(TxAll);
      Serial1.print("\t");
      Serial1.print(TxRealRev);
      Serial1.print("\t");
      Serial1.print(TxRealTime);
      Serial1.print("\t");
      Serial1.print(TxRealAmp);
      Serial1.print("\t");
      Serial1.println(TxCheck);
    }
  }
  else if (MotorSet == "Stop")
  {
    previousMillis = currentMillis;
    Speed = 0;
    analogWrite(EN, Speed);
    rev = 0;    Count = 0;
  }
}

String RxDataAll()
{
  if (Serial.available() > 0)
  {
    dataIn = Serial.read();
    waitProcess += dataIn;
    CutDataAll = waitProcess.indexOf(";");
  }

  dataAll = waitProcess.substring(0, CutDataAll);
  RxRevData(dataAll);

  return dataAll;
}

String RxRevData(String dataCut)
{
  int Cut;
  Cut = dataCut.indexOf("/");
  strRev = dataCut.substring(0, Cut);

  OutRange1 = dataCut.substring(Cut + 1);
  RxTimeData(OutRange1);

  return strRev;
}

String RxTimeData(String dataCut)
{
  int Cut;
  Cut = dataCut.indexOf("/");
  strTime = dataCut.substring(0, Cut);

  OutRange2 = dataCut.substring(Cut + 1);
  RxAmpStartData(OutRange2);

  return strTime;
}

String RxAmpStartData(String dataCut)
{
  int Cut;
  Cut = dataCut.indexOf("/");
  strAmpStart = dataCut.substring(0, Cut);

  OutRange3 = dataCut.substring(Cut + 1);
  RxAmpEndData(OutRange3);

  return strAmpStart;
}

String RxAmpEndData(String dataCut)
{
  int Cut;
  Cut = dataCut.indexOf("/");
  strAmpEnd = dataCut.substring(0, Cut);

  OutRange4 = dataCut.substring(Cut + 1);
  RxMotorStatusData(OutRange4);

  return strAmpEnd;
}

String RxMotorStatusData(String dataCut)
{
  int Cut;
  Cut = dataCut.indexOf("/");
  strMotorStatus = dataCut.substring(0, Cut);

  OutRange5 = dataCut.substring(Cut + 1);
  RxErrorData(OutRange5);

  return strMotorStatus;
}

String RxErrorData(String dataCut)
{
  int Cut;
  Cut = dataCut.indexOf(";");
  strError = dataCut.substring(0, Cut);

  return strError;
}

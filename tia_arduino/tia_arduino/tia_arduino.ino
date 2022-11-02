#include "max6675.h"

int ktcSO = 8;
int ktcCS = 9;
int ktcCLK = 10;

int role=5;
int roleDurum = 0;

MAX6675 ktc(ktcCLK, ktcCS, ktcSO);

float sicaklik;

void setup() {
  pinMode(role, OUTPUT);
  digitalWrite(role,HIGH); //role kapali
  Serial.begin(9600);
  
  delay(500);
}

void loop() {
  // basic readout test
   sicaklik = ktc.readCelsius();

   Serial.println(sicaklik);
   
   if(Serial.available() > 0)
   {
      roleDurum = Serial.read();
      
      if(roleDurum == '0')
      {
        digitalWrite(role,HIGH);
        //Serial.println(roleDurum);
      }
      else if(roleDurum == '1')
      {
        digitalWrite(role,LOW); 
        //Serial.println(roleDurum);
      }
      else
        Serial.println(sicaklik);             
   }
       
   delay(500); //minimum 500 delay
}

//
// begin license header
//
// This file is part of Pixy CMUcam5 or "Pixy" for short
//
// All Pixy source code is provided under the terms of the
// GNU General Public License v2 (http://www.gnu.org/licenses/gpl-2.0.html).
// Those wishing to use Pixy source code, software and/or
// technologies under different licensing terms should contact us at
// cmucam@cs.cmu.edu. Such licensing terms are available for
// all portions of the Pixy codebase presented here.
//
// end license header
//
// This sketch is a good place to start if you're just getting started with 
// Pixy and Arduino.  This program simply prints the detected object blocks 
// (including color codes) through the serial console.  It uses the Arduino's 
// ICSP port.  For more information go here:
//
// http://cmucam.org/projects/cmucam5/wiki/Hooking_up_Pixy_to_a_Microcontroller_(like_an_Arduino)
//
// It prints the detected blocks once per second because printing all of the 
// blocks for all 50 frames per second would overwhelm the Arduino's serial port.
//
   
#include <SPI.h>  
#include <Pixy.h>
#include <SoftwareSerial.h>
#include <SerialCommand.h>
SerialCommand sCmd;

// This is the main Pixy object 
Pixy pixy;

int x, y, w, h;

void setup()
{
  // Put your setup code here, to run once;
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  //sCmd.setDefaultHandler(errorHandler);

  pixy.init();
}

void loop()
{ 
  static int i = 0;
  int j;
  uint16_t blocks;
  char buf[100]; 
  
  // grab blocks!
  blocks = pixy.getBlocks();
  
  // If there are detect blocks, print them!
  if (blocks)
  {
    i++;
    
    // do this (print) every 50 frames because printing every
    // frame would bog down the Arduino
    if (i%20==0)
    {
      //sprintf(buf, "%d ", blocks);
      //Serial.print(buf);
      for (j=0; j<blocks; j++)
      {
        //if(pixy.blocks[j].angle)
        //{
        //  sprintf(buf, "c %d ", pixy.blocks[j].angle);
        //}
        //else
        //{
        //  sprintf(buf, "s ");
        //}
        //Serial.print(buf);

        x = pixy.blocks[j].x;
        y = pixy.blocks[j].y;
        w = pixy.blocks[j].width;
        h = pixy.blocks[j].height;
        //sprintf(buf, "%d %d %d %d %d ", pixy.blocks[j].signature, pixy.blocks[j].x, pixy.blocks[j].y, pixy.blocks[j].width, pixy.blocks[j].height);
        //sprintf(buf, "%d %d %d %d ", x, y, w, h);

        // print out to serial monitor
        //Serial.println(buf); 
        //sprintf(buf, "");
      }
      //Serial.print("end\n");
    }
  }
  else
  {
    // Reset value;
     x = 0;
     y = 0;
     w = 0;
     h = 0;
  }

  // If serial receives a 'PING', it will execite 'pingHandler'
  if (Serial.available() > 0)
    sCmd.readSerial();

  delay(50);
}

void pingHandler ()
{
  // This will write "..." on the serial port. 
  char sendToUnity[100]; 
  sprintf(sendToUnity, "%d %d %d %d", x, y, w, h);
  //Serial.println("PONG");
  Serial.println(sendToUnity);
}

void echoHandler ()
{
  char *arg;
  arg = sCmd.next();
  if (arg != NULL)
    Serial.println(arg);
  else
    Serial.println("nothing to echo");
}

using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class ArduinoConnector : MonoBehaviour
{

    /* The serial port where the Arduino is connected. */
    [Tooltip("The serial port where the Arduino is connected")]
    public string port = "COM3";
    /* The baudrate of the serial port. */
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;

    private SerialPort stream;

    public void Open()
    {
        // Opens the serial port
        stream = new SerialPort(port, baudrate);
        stream.ReadTimeout = 50;
        stream.Open();
    }

    public void WriteToArduino(string message)
    {
        // Send the request message to Arduino
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout = 0)
    {
        // Read a data from Arduino
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException)
        {
            Debug.LogError("Timeout");
            return null;
        }
    }

    public void Close()
    {
        stream.Close();
    }
}

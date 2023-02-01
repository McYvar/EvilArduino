using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class thirstyButton : MonoBehaviour
{
    SerialPort stream;
    Queue outputQueue;
    Thread thread;

    bool looping = true;

    private void Start()
    {
        StartThread();
    }

    public void StartThread()
    {
        outputQueue = Queue.Synchronized(new Queue());
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    public void ThreadLoop()
    {
        stream = new SerialPort("COM3", 9600);
        stream.ReadTimeout = 3000;
        stream.Open();

        while (IsLooping())
        {
            if (outputQueue.Count != 0)
            {
                string command = (string)outputQueue.Dequeue();
                WriteToArduino(command);
            }
        }
    }

    public void WriteToArduino(string command)
    {
        stream.WriteLine(command);
        stream.BaseStream.Flush();
    }

    public void SendToArduino(string command)
    {
        outputQueue.Enqueue(command);
    }

    public void StopThread()
    {
        lock (this)
        {
            looping = false;
        }
    }

    public bool IsLooping()
    {
        lock (this)
        {
            return looping;
        }
    }

    public void OnThirstyPress()
    {
        SendToArduino("s");
    }

    public void OnStopPress()
    {
        SendToArduino("q");
    }
}

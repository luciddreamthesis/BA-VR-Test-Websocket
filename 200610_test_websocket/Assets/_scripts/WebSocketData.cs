using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSocketData
{

	[System.Serializable]
	public class Method{
        public string jsonrpc = "2.0";
        public string method;
        public int id = 1;
        public Parameter @params = new Parameter();

	}

    [System.Serializable]
    public class Parameter
    {
        public string username;
        public string password;
        public string client_id;
        public string client_secret;
        public string _auth;
        public string status;
        public List<string> streams = new List<string>();
    }

    [System.Serializable]
    public class Response
    {
        public string jsonrpc;
        public int id = 1;
        public ResponseResult result = new ResponseResult();

    }

    [System.Serializable]
    public class ResponseResult
    {
        public string sid;
        public List<string> mot = new List<string>();
        /*IMD_COUNTER   Counter that increments by 1 each event
        IMD_GYROX   Gyroscope, X axis
        IMD_GYROY   Gyroscope, Y axis
        IMD_GYROZ   Gyroscope, Z axis
        IMD_ACCX    Acceleration, X axis
        IMD_ACCY    Acceleration, Y axis
        IMD_ACCZ    Acceleration, Z axis
        IMD_MAGX    Magnetometer, X axis
        IMD_MAGY    Magnetometer, Y axis
        IMD_MAGZ    Magnetometer, Z axis*/
        public float time;
        public List<string> met = new List<string>();
        public List<float> dev = new List<float>();
    }

}

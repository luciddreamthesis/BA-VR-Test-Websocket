using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using System.IO;
using DigitalRuby.Tween;
public class WebSocketParser : MonoBehaviour {

    public WebSocket ws;

    public delegate void OnConnected();
    public OnConnected OnConnect;
    private float x = 0.0f;
    private float lastFocusValue =0;
    private string auth; // = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhcHBJZCI6ImNvbS5taW1pLnlvcmQubHVjaWQtZHJlYW0iLCJhcHBWZXJzaW9uIjoiMS4wIiwiZXhwIjoxNTkyMDMyNDcyLCJuYmYiOjE1OTE3NzMyNzIsInVzZXJJZCI6ImFhN2E2NDI2LTUxMTAtNDc4Yy04OTFhLWYwMzlmZTlhNmEyNCIsInVzZXJuYW1lIjoibWltaS55b3JkIiwidmVyc2lvbiI6IjIuMCJ9.QNiWXsslXYqaK3wJ/aSTO6geF35jckzgJWWPC/h3AFA=";

    private Queue responseData;

    //public GameObject sphereInterest;
    //public GameObject sphereStress;
    //public GameObject sphereRelax;
    //public GameObject sphereExcitement;
    //public GameObject sphereEngagement;
    //public GameObject sphereLongTermExcitement;
    public GameObject sphereFocus;

    public Transform Target;
    float speed = 1.0f;


    //public bool isMoving = false;
    public string cortexToken;

    public float step = 4;
    float distPerStep = 1f;
    public float timePerStep = 5f;

    //public CSVExporter csvExporter;

    // Use this for initialization
    public void Main ()
    {

        responseData = new Queue();

        var wsUri = new UriBuilder();

        wsUri.Host = "localhost";
        wsUri.Port = 6868;
        wsUri.Scheme = "wss";
        // wsUri.Path = "/";

        var url = wsUri.Uri;

        Debug.Log("Opening Websocket Interaction connection to " + url.AbsoluteUri.Substring(0,url.AbsoluteUri.Length-1));
        ws = new WebSocket(url.AbsoluteUri.Substring(0, url.AbsoluteUri.Length - 1));

        ws.OnOpen += (sender, evt) => {
            Debug.Log("Open Websocket connection to");
            Authorize();

        };

        ws.OnMessage += (sender, evt) => {

            responseData.Enqueue(evt.Data);


        };

        ws.Connect();

        float distance = Vector3.Distance(Target.position, sphereFocus.transform.position);
        distPerStep = distance / step;
    }

    // Update is called once per frame
    void Update()
    {
        if (responseData.Count > 0)
        {
            var line = "";
            float lastSignalStrength = 0f;
            string responseString = responseData.Dequeue().ToString();
            Debug.Log(responseString);
            WebSocketData.ResponseResult response = JsonUtility.FromJson<WebSocketData.ResponseResult>(responseString);
            //Debug.Log(response.result);
            WebSocketData.Response responseRaw = JsonUtility.FromJson<WebSocketData.Response>(responseString);

            if(!String.IsNullOrEmpty(responseRaw.result.cortexToken)) {
              cortexToken = responseRaw.result.cortexToken;
              Debug.Log("cortexToken is: " + cortexToken);
              CreateSession(cortexToken);
            }

            if(!String.IsNullOrEmpty(responseRaw.result.id)) {
              // string sessionId = responseRaw.result.id;
              Debug.Log("session id is: " + responseRaw.result.id);
              Subscribe("met", cortexToken, responseRaw.result.id);
            //  Subscribe("met", cortexToken, responseRaw.result.id);
            }

            //if(response.mot.Count>0){
            //sphere.transform.position = new Vector3(float.Parse(response.mot[7]), float.Parse(response.mot[8]), float.Parse(response.mot[9]));
            //}
            Debug.Log (response.met[1]);


            float newFocus = float.Parse(response.met[1]);


            if(newFocus > 0.1){
                Vector3 startPos = sphereFocus.transform.position;
                
                Vector3 endPos = Vector3.MoveTowards(sphereFocus.transform.position, Target.position, distPerStep);

                

                System.Action<ITween<Vector3>> sphereMovement = (t) =>
                {
                    sphereFocus.transform.position = t.CurrentValue;
                };


                // completion defaults to null if not passed in
                sphereFocus.gameObject.Tween("SphereMovement", startPos, endPos, timePerStep, TweenScaleFunctions.CubicEaseInOut, sphereMovement);
                lastFocusValue = newFocus;
            }
            else
            {
                Debug.Log("focus down" + newFocus);
            }
            // csvExporter.WriteEEGData(response);
        }
  }

    void Authorize(){
        WebSocketData.AuthParameter p = new WebSocketData.AuthParameter();
        p.clientId = "ZFDADBiG5lri14hJjNMQSaXdooyu3x8pPDGgEbCe";
        p.clientSecret = "EiZeIaY2fAbyIeEtsbI2yjwlDuiG0WaEPpNShBC3ctnuGDMEdzULTewbIfvTY7fHguSGHjvdFcL5YZ8rFZemT8vXuSTk7IlYia2LsERvt5HS3jwNOvXpa7mzvyQXBcfO";
        SendMethodAuth("authorize", p);
    }



    void CreateSession(string cortexToken){

        WebSocketData.SessionParameter p = new WebSocketData.SessionParameter();
        p.cortexToken = cortexToken;
        p.status = "open";
        p.headset = "INSIGHT-A1D205CF";
        Debug.Log("Create session with cortexToken: " + cortexToken);
        SendMethodSession("createSession", p);
    }

    void Subscribe(string type, string cortexToken, string sessionId)
    {

        WebSocketData.SubscribeParameter p = new WebSocketData.SubscribeParameter();
        p.cortexToken = cortexToken;
        p.session = sessionId;
        p.streams.Add(type);
        Debug.Log("subscribing");
        SendMethodSubscribe("subscribe", p);
    }

    void GetUserLogin(){
        SendMethod("getUserLogin", null);
    }

    void SendLogout(){

        WebSocketData.Parameter p = new WebSocketData.Parameter();
        p.username = "mimi.yord";
        SendMethod("logout", p);
    }

    void SendLogin(){


        WebSocketData.Parameter p = new WebSocketData.Parameter();

        //p.username = "mimi.yord";
        // p.password = "Indochine_24";
        // p.client_id = "ZFDADBiG5lri14hJjNMQSaXdooyu3x8pPDGgEbCe";
        // p.client_secret = "EiZeIaY2fAbyIeEtsbI2yjwlDuiG0WaEPpNShBC3ctnuGDMEdzULTewbIfvTY7fHguSGHjvdFcL5YZ8rFZemT8vXuSTk7IlYia2LsERvt5HS3jwNOvXpa7mzvyQXBcfO";

        SendMethod("login", p);

    }

    void SendMethodSubscribe(string methodName, WebSocketData.SubscribeParameter parameter){
        WebSocketData.MethodSubscribe method = new WebSocketData.MethodSubscribe();
        method.method = methodName;
        method.@params = parameter;
        Debug.Log("method" + JsonUtility.ToJson(method));
        ws.Send(JsonUtility.ToJson(method));
    }

    void SendMethod(string methodName, WebSocketData.Parameter parameter){
        WebSocketData.Method method = new WebSocketData.Method();
        method.method = methodName;
        method.@params = parameter;
        Debug.Log("method" + JsonUtility.ToJson(method));
        ws.Send(JsonUtility.ToJson(method));
    }

    void SendMethodAuth(string methodName, WebSocketData.AuthParameter parameter){
        WebSocketData.MethodAuth method = new WebSocketData.MethodAuth();
        method.method = methodName;
        method.@params = parameter;
        Debug.Log("method" + JsonUtility.ToJson(method));
        ws.Send(JsonUtility.ToJson(method));
    }
    void SendMethodSession(string methodName, WebSocketData.SessionParameter parameter){
        WebSocketData.MethodSession method = new WebSocketData.MethodSession();
        method.method = methodName;
        method.@params = parameter;
        Debug.Log("method" + JsonUtility.ToJson(method));
        ws.Send(JsonUtility.ToJson(method));
    }
}

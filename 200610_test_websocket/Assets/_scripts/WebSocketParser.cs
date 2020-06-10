using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using System.IO;

public class WebSocketParser : MonoBehaviour {

    public WebSocket ws;

    public delegate void OnConnected();
    public OnConnected OnConnect;
    private float x = 0.0f;
    private string auth = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhcHBJZCI6ImNvbS5taW1pLnlvcmQubHVjaWQtZHJlYW0iLCJhcHBWZXJzaW9uIjoiMS4wIiwiZXhwIjoxNTkwOTIxOTIyLCJuYmYiOjE1OTA2NjI3MjIsInVzZXJJZCI6Ijk1M2MyNzhmLWEzM2QtNDRkMi05NzNhLTUzN2Q0MzQ3NDFjYSIsInVzZXJuYW1lIjoibWltaS55b3JkIiwidmVyc2lvbiI6IjIuMCJ9.aFFKQJkuykTVGcVxOuDR6I5Np4axib2jO653OUXR+6E=";


    private Queue responseData;

    public GameObject sphereInterest;
    public GameObject sphereStress;
    public GameObject sphereRelax;
    public GameObject sphereExcitement;
    public GameObject sphereEngagement;
    public GameObject sphereLongTermExcitement;
    public GameObject sphereFocus;

    public CSVExporter csvExporter;

    // Use this for initialization
    void Start()
    {

        responseData = new Queue();

        var wsUri = new UriBuilder();

        wsUri.Host = "emotivcortex.com";
        wsUri.Port = 6868;
        wsUri.Scheme = "wss";
        wsUri.Path = "wss://localhost:6868";

        var url = wsUri.Uri;

        Debug.Log("Opening Websocket Interaction connection to " + url.AbsoluteUri.Substring(0,url.AbsoluteUri.Length-1));
        ws = new WebSocket(url.AbsoluteUri.Substring(0, url.AbsoluteUri.Length - 1));

        ws.OnOpen += (sender, evt) => {
            Debug.Log("Open Websocket connection to");
            //Authorize();
            CreateSession();
            Subscribe("met");
            //Subscribe("dev");
        };

        ws.OnMessage += (sender, evt) => {

            responseData.Enqueue(evt.Data);


        };

        ws.Connect();
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

            if(response.mot.Count>0){
                //sphere.transform.position = new Vector3(float.Parse(response.mot[7]), float.Parse(response.mot[8]), float.Parse(response.mot[9]));
            }

            if(response.met.Count>0){
                float t = Time.time*0.01f;
                sphereInterest.transform.position = new Vector3(t,float.Parse(response.met[0]),0f);
                sphereStress.transform.position = new Vector3(t, float.Parse(response.met[1]), 0f);
                sphereRelax.transform.position = new Vector3(t, float.Parse(response.met[2]), 0f);
                sphereExcitement.transform.position = new Vector3(t, float.Parse(response.met[3]), 0f);
                sphereEngagement.transform.position = new Vector3(t, float.Parse(response.met[4]), 0f);
                sphereLongTermExcitement.transform.position = new Vector3(t, float.Parse(response.met[5]), 0f);
                sphereFocus.transform.position = new Vector3(t, float.Parse(response.met[6]), 0f);
   
                csvExporter.WriteEEGData(response);
            }
           
        }
    }
    void Authorize(){
        WebSocketData.Parameter p = new WebSocketData.Parameter();
        SendMethod("authorize", p);
    }


    void CreateSession(){

        WebSocketData.Parameter p = new WebSocketData.Parameter();
        p._auth = auth;
        p.status = "open";
        SendMethod("createSession", p);
    }

    void Subscribe(string type)
    {

        WebSocketData.Parameter p = new WebSocketData.Parameter();
        p._auth = auth;
        p.streams.Add(type);
        SendMethod("subscribe", p);
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

        p.username = "mimi.yord";
        p.password = "Indochine_24";
        p.client_id = "ZFDADBiG5lri14hJjNMQSaXdooyu3x8pPDGgEbCe";
        p.client_secret = "EiZeIaY2fAbyIeEtsbI2yjwlDuiG0WaEPpNShBC3ctnuGDMEdzULTewbIfvTY7fHguSGHjvdFcL5YZ8rFZemT8vXuSTk7IlYia2LsERvt5HS3jwNOvXpa7mzvyQXBcfO";

        SendMethod("login", p);

    }

    void SendMethod(string methodName, WebSocketData.Parameter parameter){
        WebSocketData.Method method = new WebSocketData.Method();
        method.method = methodName;
        method.@params = parameter;
        Debug.Log("method" + JsonUtility.ToJson(method));
        ws.Send(JsonUtility.ToJson(method));
    }
}

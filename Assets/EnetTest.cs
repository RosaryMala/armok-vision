using UnityEngine;
using System.Collections;
using System.Net;

public class EnetTest : MonoBehaviour {
    public const int Port = 29292;

    private ENet.Host serverHost;
    private ENet.Host clientHost;
    private ENet.Peer client;

    private bool connected;

	// Use this for initialization
	void Start () {
        Debug.Log("INIT");
        ENet.Library.Initialize();

        serverHost = new ENet.Host ();
        serverHost.InitializeServer (Port, 1);

        clientHost = new ENet.Host ();
        clientHost.InitializeClient (1);

        client = clientHost.Connect (new IPEndPoint(IPAddress.Loopback, Port), 1234);

        connected = false;
	}
	
	// Update is called once per frame
	void Update () {
        ENet.Event e;

        while (clientHost.Service (0, out e)) {
            Debug.Log ("At frame " + Time.frameCount + ", client event: " + e.Type);
            connected = true;
        }
        while (serverHost.Service (0, out e)) {
            Debug.Log ("At frame " + Time.frameCount + ", server event: " + e.Type);
            if (e.Type == ENet.EventType.Receive) {
                Debug.Log("Payload: " + e.Packet.GetBytes()[0]);
            }
        }

        if (connected && Time.frameCount % 60 == 0) {
            ENet.Packet p = new ENet.Packet();
            p.Initialize (new byte[] {(byte) Time.fixedTime});
            client.Send (0, p);
        }
	}

    void OnDestroy () {
        Debug.Log("DEINIT");
        clientHost.Dispose();
        serverHost.Dispose();
        ENet.Library.Deinitialize();
    }
}

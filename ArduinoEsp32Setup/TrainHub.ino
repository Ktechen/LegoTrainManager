#include <WiFi.h>
#include <WebServer.h>
#include <Arduino.h>
#include "Lpf2Hub.h"

// Network credentials
const char* password = "";

// Create a web server on port 80
WebServer server(80);

enum TrainState {
  IDLE,
  DRIVING,
  STOPPED,
};

const char* trainStateToString(TrainState state) {
    switch (state) {
        case IDLE:
            return "Idle";
        case DRIVING:
            return "Driving";
        case STOPPED:
            return "Stopped";
        default:
            return "Unknown State";
    }
}


TrainState currentTrainState = IDLE;

// Create a hub instance
Lpf2Hub myTrainHub;
byte port = (byte)PoweredUpHubPort::A;

void setup() {
  Serial.begin(115200);
  // Connect to Wi-Fi
  wifiSetup();
  // Setup web server routes
  setupServer();
}

void wifiSetup() {
  // Start WiFi in station mode and disconnect any previous connection
  WiFi.mode(WIFI_STA);
  WiFi.disconnect();
  delay(100);

  Serial.println("Scanning for networks...");
  int n = WiFi.scanNetworks();
  Serial.println("Scan done.");
  if (n == 0) {
    Serial.println("No networks found.");
    return;  // Exit if no networks found
  }

  // Connect to the first network found 
  String ssid = WiFi.SSID(0);
  Serial.print("Connecting to ");
  Serial.println(ssid);

  // Connect using a global password
  WiFi.begin(ssid.c_str(), password);  // Ensure 'password' is defined globally

  // Wait for connection
  int connectTries = 0;
  while (WiFi.status() != WL_CONNECTED && connectTries < 10) {  // Increased the number of retries to 10
    delay(500);
    Serial.print(".");
    connectTries++;
  }

  // Check connection status
  if (WiFi.status() == WL_CONNECTED) {
    Serial.println("\nConnected!");
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
  } else {
    Serial.println("\nFailed to connect to the first network.");
  }
}

void trainLoop() {
  if (!myTrainHub.isConnected() && !myTrainHub.isConnecting()) {
    myTrainHub.init();  // initalize the PoweredUpHub instance
    //myTrainHub.init("90:84:2b:03:19:7f"); //example of initializing an hub with a specific address
  }

  // connect flow. Search for BLE services and try to connect if the uuid of the hub is found
  if (myTrainHub.isConnecting()) {
    myTrainHub.connectHub();
    if (myTrainHub.isConnected()) {
      Serial.println("Connected to HUB");
      Serial.print("Hub address: ");
      Serial.println(myTrainHub.getHubAddress().toString().c_str());
      Serial.print("Hub name: ");
      Serial.println(myTrainHub.getHubName().c_str());
    } else {
      Serial.println("Failed to connect to HUB");
    }
  }
}

void setupServer() {
  server.on("/", HTTP_GET, []() {
    server.send(200, "text/plain", "Welcome to train api :)");
  });

  server.on("/currentTrainState", HTTP_GET, []() {
    server.send(200, "text/plain", trainStateToString(currentTrainState));
  });

  // POST request to set speed
  server.on("/speed", HTTP_POST, []() {
    if (server.hasArg("speed")) {
      int speed = server.arg("speed").toInt();
      if (speed == 0) {
        setTrainSpeed(speed);
        currentTrainState = STOPPED;
      } else {
        setTrainSpeed(speed);
        currentTrainState = DRIVING;
      }
    } else {
      server.send(400, "text/plain", "Missing speed parameter");
    }
  });

  // GET request to getHubName
  server.on("/hubName", HTTP_GET, []() {
    if (myTrainHub.isConnected()) {
      std::string item = myTrainHub.getHubName();
      Serial.println(item.c_str());
      server.send(200, "text/plain", item.c_str());
    } else {
      server.send(500, "text/plain", "Hub not connected");
    }
  });



  // GET request to stop the train
  server.on("/stop", HTTP_GET, []() {
    if (myTrainHub.isConnected()) {
      setTrainSpeed(0);
      currentTrainState = STOPPED;
      Serial.println("Train stopped");
      server.send(200, "text/plain", "Train stopped");
    } else {
      Serial.println("Hub not connected. Cannot stop train.");
      server.send(500, "text/plain", "Hub not connected");
    }
  });

  // Endpoint for setting LED color via RGB
  server.on("/setRGB", HTTP_POST, []() {
    if (server.hasArg("r") && server.hasArg("g") && server.hasArg("b")) {
      byte r = server.arg("r").toInt();
      byte g = server.arg("g").toInt();
      byte b = server.arg("b").toInt();
      setLedColorRGB(r, g, b);
    } else {
      server.send(400, "text/plain", "Missing RGB parameters");
    }
  });

  server.begin();
  Serial.println("HTTP server started");
}

void setTrainSpeed(int speed) {
  if (myTrainHub.isConnected()) {
    myTrainHub.setBasicMotorSpeed(port, speed);
    Serial.print("Speed set to: ");
    Serial.println(speed);
    server.send(200, "text/plain", "Speed set to " + String(speed));
  } else {
    Serial.println("Hub not connected. Cannot set speed.");
    server.send(500, "text/plain", "Hub not connected");
  }
}

void setLedColorRGB(byte r, byte g, byte b) {
  myTrainHub.setLedRGBColor(r, g, b);  // Adjust if the library supports direct RGB setting
  Serial.print("LED set to RGB: ");
  Serial.print(r);
  Serial.print(", ");
  Serial.print(g);
  Serial.print(", ");
  Serial.println(b);
  server.send(200, "text/plain", "LED set to RGB(" + String(r) + ", " + String(g) + ", " + String(b) + ")");
}

void loop() {
  server.handleClient();
  trainLoop();
  delay(20);
}

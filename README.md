# LegoTrainManager

- Documentation and quick start LegoHub: https://github.com/corneliusmunz/legoino

## How to start

- install [Arduino IDE](https://www.arduino.cc/en/software)
- Connect esp32
- Select COM Port
- Download ArduinoEsp32Setup/TrainHub.ino
- Select .ino and change Network credentials
- Start deploy
  
## Basic:
- [x] Connect train with esp32
- [x] start web server on esp32 to call via http
- [x] create drive, stop and rgb mode via http  
- [x] create asp.net core web server to impl. http calls
- [x] create frontend to handle train options
- [x] add move forward/backward
- [x] add brake 
- [x] add brake slowly 
- [ ] change LED color [Blink|Perm|Off]
- [ ] get battery status
- [x] get information is esp32 connected/disconnected
- [ ] get information is train connected/disconnected

## network
- [ ] add esp32 to local dns
- [ ] move esp32 to powerbank or battery (need to be calulate)
- [ ] deploy on zimaboard/raspberry
      
## Advance
- [ ] Add dashboard looks like real train interface
- [ ] optimize performance for esp32 request/response webserver
- [ ] connect multi trains 
- [ ] add train control to twitch api 
- [ ] map Interface (2D or 3D):

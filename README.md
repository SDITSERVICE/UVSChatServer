## Universal Voice SignalR Chat - Server
---
*  __Server mode__ 
  
*Temporarily used only to send an audio sample to all clients except yourself*



---
what is Universal Voice SignalR Chat.
This is an implementation of voice (and written) chat. The implementation takes place thanks to the SiganalR library, which allows you to use this chat anywhere.
In order for it to work, we need to receive data from the microphone in the form of a byte array and send it over the network.

In order to try how it works you need to download SampleClient in the RELEASES section
---

|   Feature    |        Support        |  Comment |
|----------| :----------------:|------:|
| NO_DATA |√||
| NO_DATA |Х|NO_DATA|
| NO_DATA |√ / Х|NO_DATA|

---

Config project

replace config file `appsettings.json`

```json
{
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---



TODO LIST:
- [x] Sending a byte array to other participants
- [x] Sending a float array to other participants
- [ ] Sending a written message
- [ ] Send audio to a specific person
- [ ] Sending audio to a specific group

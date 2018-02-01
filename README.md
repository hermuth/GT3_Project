# GT3_Project

## How To Use

Beim Start (auch aus dem Editor herraus) ggf. Firewallausnahmen anpassen / Firewall deaktivieren.

Nutzbar mit jedem aktuellen Smartphone.
Nach dem Start der Anwendung, im Browser des Smartphones die IP (3. von oben) eingeben und mit der Anwendung verbinden.
Smartphone so halten das das Display zu einem zeigt.
Nach vorne kippen zu beschleunigen, nach hinten kippen zum bremsen / rückwärtsfahren.
Links, rechts drehen zum lenken. Sollte die Lenkachse verkehrt sein, das Smartphone einmal um 180° drehen
(Das Display zeigt immer noch zu einem.).

## Remote Controll Modul

Folgende Scripte sind für das Remote Controll Modul nötig:

- RemoteControll.cs
- WebServices.cs

WebServices.cs regelt die Kommunication Zwischen Smartphone und der Unity Anwendung.
RemoteControll.cs interpretiert die Daten die vom WebServices.cs empfangen wurden.

## Remote Controll Modul

Folgende Scripte sind für das Remote Controll Modul nötig:

- RemoteControll.cs
- WebServices.cs

WebServices.cs regelt die Kommunication Zwischen Smartphone und der Unity Anwendung.
RemoteControll.cs interpretiert die Daten die vom WebServices.cs empfangen wurden.

## Known Issues

- Lenkenachse-Maxima kehrt sich um beim bremsen / rückwärtsfahren
- Kein neustart zur Laufzeit möglich (man muss die Anwendung neu starten)
# Scen Setup Guide - Run4theRelic

## Förutsättningar
- Unity 2022.3 LTS installerat
- Projektet öppnat i Unity Hub
- OpenXR aktiverat för PC

## Steg 1: Aktivera OpenXR
1. Gå till **Edit > Project Settings**
2. Välj **XR Plug-in Management**
3. Klicka **Install XR Plug-in Management** om det inte redan är installerat
4. Under **PC Settings**, aktivera **OpenXR**
5. Klicka **Install OpenXR Package** om det behövs

## Steg 2: Importera XRI Samples
1. Öppna **Window > Package Manager**
2. Sök efter **XR Interaction Toolkit**
3. Klicka **Import** på **XR Interaction Toolkit Samples**
4. Välj alla samples och klicka **Import**

## Steg 3: Skapa XR Origin
1. Högerklicka i **Hierarchy** → **XR > XR Origin (Action-based)**
2. Detta skapar:
   - XR Origin
   - Main Camera (XR)
   - LeftHand Controller
   - RightHand Controller

## Steg 4: Skapa Pussel-rum
1. **Room 1 (CableConnect)**
   - Skapa en tom GameObject, döp till "CableConnectRoom"
   - Lägg till `CableConnectController` script
   - Skapa 3-4 `CableSocket` GameObjects
   - Skapa 3-4 `CablePlug` GameObjects

2. **Room 2 (RunicSequence)**
   - Skapa "RunicSequenceRoom"
   - Lägg till `RunicSequenceController` script
   - Skapa 4 `RunicPad` GameObjects

3. **Room 3 (BalanceBridge)**
   - Skapa "BalanceBridgeRoom"
   - Lägg till `BalanceBridgeController` script
   - Skapa en `testPoint` GameObject

## Steg 5: Lägg till Relic och Extraction
1. **Relic Arena**
   - Skapa "RelicArena" GameObject
   - Lägg till `RelicController` script
   - Skapa en "Relic" GameObject som child

2. **Extraction Zone**
   - Skapa "ExtractionZone" GameObject
   - Lägg till `ExtractionZone` script
   - Lägg till en Collider (Trigger)

## Steg 6: Koppla Scripts
1. **MatchOrchestrator**
   - Lägg till `MatchOrchestrator` script på en tom GameObject
   - Koppla alla pussel-controllers i Inspector

2. **SabotageManager**
   - Lägg till `SabotageManager` script på en tom GameObject

3. **PlayerMovementHook**
   - Lägg till `PlayerMovementHook` script på XR Origin
   - Koppla till ContinuousMoveProvider i Inspector

## Steg 7: Testa
1. Klicka **Play** i Unity
2. Verifiera att:
   - XR Origin rör sig med WASD
   - Scripts kompilerar utan fel
   - Console visar inga errors

## Felsökning
- **Scripts kompilerar inte**: Kontrollera att alla using-statements finns
- **XR Origin rör sig inte**: Verifiera att OpenXR är aktiverat
- **Missing references**: Kontrollera att alla scripts är kopplade i Inspector

## Nästa Steg
Efter setup, se `Documentation/TASKS.md` för MVP-uppgifter. 

## World-space HUD
Så här lägger du till och testar HUD som visar fas, timer och sabotage-meddelanden:

1. Lägg till ett tomt GameObject i scenen och döp det till `HUD`.
2. Lägg till komponenten `HUDController` på `HUD`-objektet.
   - Den skapar sina world-space texter själv vid runtime.
   - Den försöker använda TextMeshPro om det finns installerat, annars används vanlig `TextMesh` (3D-text).
   - För att lägga till TMP: öppna Package Manager och installera "TextMeshPro".
3. Justera i Inspector vid behov:
   - `distance` (standard 1.2 m framför kameran)
   - `localOffset` (standard 0.25 m upp)
   - `timerColor` (standard grön→gul→röd)
4. Testa i Play-läge:
   - Vid start: se fasen "COUNTDOWN" och en nedräkning (3..2..1).
   - I Pussel 1–3: se timer ticka ner och färgskala från grön till röd.
   - Trigga `SabotageManager.ApplyFog()` (via debug-knapp eller Inspector) för att se rött "SABOTAGED!" som flashar i ~2 s och fade:ar ut.

Unity-teststeg (utan TMP och med TMP):
- Öppna projektet, se att scenen har XR Origin + pussel.
- Lägg tomt GO `HUD` och add `HUDController`.
- Play → se fas och timer uppdateras; lös Pussel 1–3 och se färgskift.
- Anropa `ApplyFog()` i `SabotageManager` → se röd "SABOTAGED!" flash som fade:ar ut.
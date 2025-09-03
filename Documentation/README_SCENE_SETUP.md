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

## Laser Redirect (demo)
Så här kör du den inbyggda demobyggaren och testar pusslet:

1. Öppna en tom scen eller valfri testszen.
2. Gå till menyn: `Relic > Build Laser Redirect (demo)`.
3. Detta skapar ett litet demo-rum under `LaserRedirect_Demo` med:
   - **LaserEmitter** med röd `LineRenderer`
   - **2 speglar** (Cubes med `LaserMirror` + Collider)
   - **LaserReceiver** (Cube med `LaserReceiver`)
   - **LaserRedirectController** länkad till receivern
4. Se till att `LaserEmitter.hitMask` står på `Default` (sätts av buildern).
5. Kör scenen. Rota/greppa speglarna (XRI hanterar manipulering) för att styra laserstrålen.
6. Håll lasern på `LaserReceiver` tills dess interna timer (1.5s) fylls. När det sker ropas controllerns completion och pusslet blir clearat.
7. (Valfritt) Lägg till `LaserDebugGizmo` på emittern för att rita små sfärer vid träffpunkter.
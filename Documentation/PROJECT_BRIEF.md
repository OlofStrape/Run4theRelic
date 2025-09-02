# Projektbrief - Run4theRelic

## Mål
Skapa en VR-arenapussel där spelare tävlar genom en serie utmaningar för att nå Relic:en och extrahera den.

## Core Loop
1. **Lobby** - Spelare ansluter och väntar
2. **Countdown** - 3-2-1 start
3. **Puzzle 1-3** - Sekventiella barriärpussel
4. **Gold Time Sabotage** - Fog-effekt för att försvåra
5. **Relic Final** - Relic:en spawnas, sista striden
6. **Extraction** - Vinnaren extraherar Relic:en
7. **Post Match** - Resultat och statistik

## Icke-mål
- Multiplayer i MVP (M3)
- Komplexa 3D-modeller
- Ljudsystem
- AI-motståndare

## Constraints
- Unity 2022.3 LTS
- URP för prestanda
- OpenXR för kompatibilitet
- Matchtid 5-8 minuter
- Endast textfiler i versionering

## Designprinciper
- **Enkelhet** - Tydliga pussel med intuitiva mekaniker
- **Balans** - Jämn svårighetsgrad genom alla faser
- **Feedback** - Tydlig respons på alla spelaråtgärder
- **Skalbarhet** - Enkel att lägga till nya pussel
- **Testbarhet** - Kompilerbar kod med tydliga API:er 
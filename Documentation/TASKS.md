# Uppgifter och Milestones - Run4theRelic

## M1: MVP (Minimal Viable Product)
**Tidsram**: 2-3 veckor
**Mål**: Grundläggande spelmekaniker och pussel

### Core System
- [ ] GameEvents-system implementerat
- [ ] MatchOrchestrator med alla matchfaser
- [ ] Grundläggande scenstruktur

### Pussel
- [ ] CableConnect - Färgmatchande kabelpussel
- [ ] RunicSequence - Simon-sekvenspussel  
- [ ] BalanceBridge - Balanspussel med timer
- [ ] Timer-system med gold-time logik

### Player & Relic
- [ ] PlayerMovementHook för XRI-integration
- [ ] RelicController med pickup/drop
- [ ] ExtractionZone för vinst

### Sabotage
- [ ] SabotageManager med fog-effekt
- [ ] Gold-time sabotage-trigger

### Acceptance M1
- [ ] Alla scripts kompilerar utan fel
- [ ] Grundläggande pussel-logik fungerar
- [ ] Matchfaserna avancerar korrekt
- [ ] Relic kan plockas upp och extraheras
- [ ] Player-rörelse fungerar med XRI

## M2: Polish
**Tidsram**: 1-2 veckor
**Mål**: Visuella förbättringar och UX

### UI/UX
- [ ] GameUI med match-status
- [ ] PuzzleUI för varje pussel
- [ ] Timer-visualisering
- [ ] Feedback-animationer

### Game Feel
- [ ] Ljud-effekter för pussel
- [ ] Partikelsystem för feedback
- [ ] Smooth transitions mellan faser
- [ ] Visual feedback för sabotage

### Acceptance M2
- [ ] UI är intuitivt och responsivt
- [ ] Visuella effekter förbättrar gameplay
- [ ] Feedback är tydligt och tillfredsställande

## M3: Nätverk
**Tidsram**: 2-3 veckor
**Mål**: Multiplayer-funktionalitet

### Multiplayer Core
- [ ] Spelare kan ansluta till lobby
- [ ] Synkroniserad match-start
- [ ] Real-time pussel-status
- [ ] Spelare kan eliminera varandra

### Nätverksarkitektur
- [ ] Client-server kommunikation
- [ ] Lag-synkronisering
- [ ] Anti-cheat grundstruktur
- [ ] Reconnection-hantering

### Acceptance M3
- [ ] 2+ spelare kan spela samtidigt
- [ ] Pussel-status synkroniseras
- [ ] Relic-possession synkroniseras
- [ ] Match-resultat sparas

## Prioritering
1. **Högsta prioritet**: M1 MVP - Grundläggande funktionalitet
2. **Medium prioritet**: M2 Polish - Användarupplevelse
3. **Lägsta prioritet**: M3 Nätverk - Multiplayer

## Definition of Done
- [ ] Kod kompilerar utan fel
- [ ] Funktioner testade i Unity
- [ ] Dokumentation uppdaterad
- [ ] Code review genomförd
- [ ] Acceptance-kriterier uppfyllda 
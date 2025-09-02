# Ordlista - Run4theRelic

## Speltermer

### Zon
- **Lobby**: Väntezon innan match startar
- **Puzzle Room**: Rum med specifikt pussel
- **Relic Arena**: Slutgiltig stridszon
- **Extraction Zone**: Vinstzon för Relic-extraktion

### Gate
- **Barrier Gate**: Lås som öppnas när pussel är löst
- **Progress Gate**: Kontrollpunkt mellan pussel-faser
- **Final Gate**: Sista barriären innan Relic Arena

### Gold Time
- **Gold Time**: Optimal tid för att lösa pussel (50% av timeLimit)
- **Gold Time Sabotage**: Fog-effekt som aktiveras vid gold time
- **Sabotage Duration**: 5 sekunder fog-effekt

### Sabotage
- **Fog Effect**: Visuell försvåring som aktiveras vid gold time
- **Sabotage Trigger**: Automatisk aktivering baserat på pussel-tid
- **Clear Sabotage**: Fog-effekten avaktiveras efter duration

### Relic
- **Relic**: Huvudobjekt som ska extraheras för vinst
- **Relic Spawn**: Relic:en skapas vid start av Final-fas
- **Relic Pickup**: Spelare kan plocka upp Relic:en
- **Relic Drop**: Relic:en släpps vid hård kollision
- **Relic Extraction**: Relic:en tas till Extraction Zone

### Extraction
- **Extraction Zone**: Specifik zon där Relic kan extraheras
- **Extraction Time**: Tid det tar att extrahera Relic:en
- **Victory Condition**: Relic extraherad = match vunnen

## Tekniska Termer

### XRI (XR Interaction Toolkit)
- **XR Origin**: Huvudspelar-objekt med kamera och controllers
- **Action-based**: Input-system baserat på Unity Input System
- **ContinuousMoveProviderBase**: Komponent för kontinuerlig rörelse

### Pussel-system
- **PuzzleControllerBase**: Basklass för alla pussel
- **Time Limit**: Maximal tid för att lösa pussel
- **Gold Time Fraction**: Procentandel av timeLimit för gold time
- **Completion Event**: Event som triggas när pussel är löst

### Match-faser
- **Lobby**: Vänteläge
- **Countdown**: 3-2-1 nedräkning
- **Puzzle1-3**: Sekventiella pussel-faser
- **GoldTimeSabotage**: Sabotage-fas
- **Final**: Relic-spawning och sista strid
- **PostMatch**: Resultat och statistik

### API-kontrakt
- **GameEvents**: Statiska events för systemkommunikation
- **MatchOrchestrator**: Hanterar match-progression
- **PlayerMovementHook**: Wrapper för XRI-rörelse
- **SabotageManager**: Hanterar fog-effekter 
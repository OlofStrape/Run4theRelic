# AI-Agent Regler för Run4theRelic

## Tillåtna Ändringar
AI-agenter får **ENDAST** ändra följande filtyper:
- `.cs` - C#-scripts
- `.md` - Markdown-dokumentation
- `.json` - Konfigurationsfiler
- `.yaml` - YAML-konfigurationer

## Förbjudna Ändringar
AI-agenter får **ALDRIG** ändra:
- `.unity` - Unity-scener
- `.prefab` - Prefab-filer
- `.meta` - Unity metadata
- `ProjectSettings/` - Binära Unity-inställningar
- `Library/`, `Temp/`, `Obj/`, `Build/` - Unity-cache

## Commit-stil
Använd konventionell commit-stil:
```
feat(core): lägg till GameEvents-system
feat(puzzles): implementera CableConnect-pussel
feat(relic): skapa RelicController
docs: uppdatera API-kontrakt
fix(player): korrigera PlayerMovementHook
```

## PR-riktlinjer
- **Små PR:er** - Max 5-10 filer per PR
- **Tydlig beskrivning** - Vad, varför och hur
- **Acceptance-checklista** - Verifiera alla krav
- **Inga binära filer** - Endast textfiler i diff

## Acceptance-checklista
- [ ] Kompilerar i Unity 2022.3 LTS
- [ ] Endast textfiler i diff
- [ ] Publika API:er följer dokumentation
- [ ] Kod har XML-dokumentation
- [ ] Teststeg dokumenterade 
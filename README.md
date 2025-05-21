# 🌿 Lieblingskind der Natur

**Ein evolutionäres Strategiespiel über genetische Manipulation, ökologische Balance und indirekte Kontrolle.**

> In einer simulierten Welt kämpfen Elefanten, Mäuse und Schlangen ums Überleben. Jede Spezies hat ihre natürlichen Feinde – aber du als "Mutter Natur" hast ein Lieblingskind. Deine Aufgabe: Sorge dafür, dass deine Spezies dominiert – durch Mutation, Umweltmanipulation und langfristige Strategie.

---

## 🛜 Installations-Guide

1. Lade den Build Ordner herunter [`Builds`](Builds).
2. Extrahiere die ZIP-Datei.
3. Im extrahierten Ordner findest du eine ausführbare Datei namens "CVCampProjekt.exe".
4. Starte diese Datei, um das Spiel zu starten und den Spaß zu beginnen.
   
---

## 🎮 Spielprinzip

- 🐘 **Elefanten** zertreten Schlangen  
- 🐍 **Schlangen** fressen Mäuse  
- 🐭 **Mäuse** sabotieren Elefanten  

Ein erweitertes **Rock-Paper-Scissors-Ökosystem**, gesteuert über Genetik und Umweltintervention.

---

## 🧬 Features

- **Indirekte Steuerung**: Du kontrollierst keine Einheiten direkt – stattdessen beeinflusst du Gene, Nahrung und Ökosysteme.
- **Mutationen**: Investiere Evolutionspunkte in genetische Veränderungen deiner Spezies – oder manipuliere die Schwächen deiner Gegner.
- **Dynamisches Gleichgewicht**: Das System reagiert auf Überpopulation, Ressourcenknappheit und Ungleichgewicht.
- **Verhalten & Sensorik**: Tiere erkennen Nahrung oder Partner basierend auf Sichtbereich und Genetik.

---

## 🔧 Technische Details

- Entwickelt mit **Unity**
- Programmiert in **C#**
- KI-Logik über **Finite State Machines** und **Urges**
- Umweltanalyse mit **spatial queries** (Sichtfeld, Positionserkennung)
- Reaktive Genetiksysteme zur Echtzeit-Mutation

> 🔍 **Der vollständige Quellcode befindet sich unter:**  
> [`Assets/Scripts`](Assets/Scripts)

---

## 📁 Projektstruktur (Auszug)

```text
Assets/
├── Scripts/             # Hauptquellcode (Genetik, AI, Umwelt, Evolution)
├── Prefabs/             # Tiere, Pflanzen, UI-Elemente
├── Materials/           # Optik

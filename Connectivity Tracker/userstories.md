

# Executive Summary: Windows Connectivity Tracker

### Ausgangslage & Problemstellung

In einer zunehmend mobilen und digitalen Arbeitswelt ist eine stabile Internetverbindung die kritischste Ressource. Nutzer – ob im Homeoffice, im Zug oder im Café – stehen häufig vor dem Problem schwankender Verbindungsqualität. Oft ist unklar, ob das Problem beim Provider, am aktuellen Standort oder an der eigenen Auslastung liegt. Es fehlt an Werkzeugen, die diese Daten nicht nur messen, sondern auch mit dem geografischen Standort und dem Nutzungskontext verknüpfen.

### Projektziel

Ziel dieses Projektes ist die Entwicklung einer nativen Windows-Applikation („Connectivity Tracker“), die als intelligenter Hintergrunddienst agiert. Das Tool soll Transparenz über die Netzqualität schaffen, indem es technische Metriken mit realen Welt-Daten (Standort, Umgebung) korreliert und den Nutzer proaktiv bei Veränderungen informiert.
### Kernfunktionen (Key Features)

Das System stützt sich auf vier funktionale Säulen:

1. **Echtzeit-Diagnose:** Kontinuierliche Überwachung der Latenz (Ping zu Referenzservern) und des lokalen Datendurchsatzes, um Engpässe sofort zu identifizieren.
2. **Location Intelligence:** Verknüpfung von Leistungsdaten mit GPS-Koordinaten, um „Funklöcher“ auf einer Karte visualisierbar zu machen.
3. **Kontext-Bewusstsein:** Manuelle Kategorisierung der Umgebung (z. B. „Im Zug“, „Homeoffice“, „Hotel“), um statistische Vergleiche zwischen verschiedenen Arbeitsorten zu ermöglichen.
4. **Proaktive Warnsysteme:** Intelligente Benachrichtigungen, die den Nutzer warnen, _bevor_ die Verbindung komplett abreißt (z. B. bei steigender Latenz), und informieren, sobald die Leitung wieder stabil ist.
    

### Nutzenversprechen (Value Proposition)

Für den Endanwender wandelt das Tool das abstrakte Gefühl von „schlechtem Internet“ in harte Fakten um.

- **Planbarkeit:** Der Nutzer wird gewarnt, wichtige Arbeit zu speichern, wenn die Verbindung instabil wird.
- **Beweisbarkeit:** Historische Daten dienen als Beleg gegenüber Internetanbietern bei Störungen.
- **Erkenntnisgewinn:** Der Nutzer lernt, an welchen Orten oder in welchen Verkehrsmitteln produktives Arbeiten möglich ist.



### Epic 0: UI-Grundgerüst & App-Skelett

**Story 0.1: Hauptfenster (Main Shell)**

> **Als** Nutzer **möchte ich** ein modernes Anwendungsfenster öffnen können, das als Rahmen für alle zukünftigen Inhalte dient, **damit** ich einen zentralen Einstiegspunkt für die Nutzung des Tools habe.

- **Akzeptanzkriterien:**
    - Das Fenster verfügt über Standard-Windows-Steuerelemente (Minimieren, Maximieren, Schließen).
    - Der Titel der Anwendung wird in der Titelleiste angezeigt.
    - Das Fenster hat eine definierte Mindestgröße, um Darstellungsfehler zu vermeiden.
    - Das Design nutzt ein modernes Framework (z. B. WPF, WinUI 3 oder .NET MAUI) für einen zeitgemäßen Look.

**Story 0.2: Navigationsstruktur**

> **Als** Nutzer **möchte ich** über eine Seitenleiste oder Tabs zwischen den verschiedenen Hauptbereichen (Dashboard, Historie, Einstellungen) wechseln können, **damit** ich später schnell auf die entsprechenden Funktionen zugreifen kann.

- **Akzeptanzkriterien:**
    - Eine Navigationsleiste (z. B. linksseitig) ist implementiert.
    - Es gibt Platzhalter-Buttons für „Dashboard“, „Verlauf“ und „Einstellungen“.
    - Beim Klick auf einen Menüpunkt wechselt der Hauptinhaltsbereich (Content Region) auf eine leere Platzhalter-Ansicht des jeweiligen Bereichs (Routing funktioniert).
        

**Story 0.3: System Tray Integration (Infobereich)**

> **Als** Nutzer **möchte ich**, dass sich die Anwendung in den Windows System Tray (Bereich neben der Uhr) minimieren lässt, anstatt geschlossen zu werden, **damit** das Tool später unauffällig im Hintergrund laufen kann, ohne Platz in der Taskleiste zu verbrauchen.

- **Akzeptanzkriterien:**
    
    - Beim Klick auf das „X“ (Schließen) wird die App nicht beendet, sondern in den Tray minimiert.
    - Es gibt ein App-Icon im System Tray.
    - Ein Doppelklick auf das Tray-Icon öffnet das Hauptfenster wieder.
    - Ein Rechtsklick auf das Tray-Icon öffnet ein Kontextmenü mit der Option „Beenden“, um die App vollständig zu schließen.
        

**Story 0.4: Dashboard-Layout (Wireframe)**

> **Als** Nutzer **möchte ich** auf der Startseite (Dashboard) bereits die grobe Aufteilung der Elemente sehen (Platzhalter für Graphen, Platzhalter für aktuelle Werte, Platzhalter für Location-Tag), **damit** die visuelle Hierarchie klar ist, bevor die echte Datenanbindung erfolgt.

- **Akzeptanzkriterien:**
    - Das Layout ist in logische Zonen unterteilt (z. B. "Live-Status" oben groß, "Verlauf" unten kleiner).
    - Die UI ist "Responsive": Wenn ich das Fenster größer ziehe, passen sich die Platzhalter-Boxen sinnvoll an die neue Größe an.


### Epic 1: Netzwerk-Monitoring (Ping & Traffic)

**Story 1.1: Regelmäßiger Ping**

> **Als** Nutzer **möchte ich**, dass das Tool in konfigurierbaren Abständen automatisch einen Ping an einen zuverlässigen Server (z. B. Google DNS) sendet, **damit** ich die aktuelle Latenz (Verzögerung) meiner Internetverbindung überwachen kann.

- **Akzeptanzkriterien:**
    - Der Ping erfolgt im Hintergrund ohne Blockieren der UI.
    - Der Nutzer kann das Intervall einstellen (z. B. alle 5, 10 oder 60 Sekunden).
    - Die Antwortzeit (ms) wird gespeichert.
        

**Story 1.2: Traffic-Überwachung**

> **Als** Nutzer **möchte ich** den aktuellen Datendurchsatz (Upload und Download) meines gesamten Systems in Echtzeit sehen, **damit** ich einschätzen kann, ob eine langsame Verbindung an der Leitung oder an meiner eigenen Bandbreitennutzung liegt.

- **Akzeptanzkriterien:**
    - Das Tool liest die Bytes Sent/Received der aktiven Netzwerkkarte aus.
    - Die Anzeige erfolgt in verständlichen Einheiten (KB/s, MB/s).

---

### Epic 2: Standort & Kontext

**Story 2.1: Standort-Tracking**

> **Als** mobiler Nutzer **möchte ich**, dass meine Verbindungsdaten automatisch mit meinen GPS-Koordinaten (oder Windows Location Service) verknüpft werden, **damit** ich später auf einer Karte sehen kann, an welchen geografischen Orten ich Funklöcher hatte.

- **Akzeptanzkriterien:**
    
    - Abfrage der Windows Location API.
    - Speicherung von Längen- und Breitengrad zu jedem Messpunkt.
    - Fallback-Option, falls kein GPS verfügbar ist (z. B. "Standort unbekannt").
        

**Story 2.2: Kontext-Kategorisierung (Tagging)**

> **Als** Nutzer **möchte ich** aus vordefinierten Kategorien (Zug, Auto, Zuhause, Café, Unterwegs) meinen aktuellen Status auswählen können, **damit** ich statistisch auswerten kann, welches Verkehrsmittel oder welcher Ort die stabilste Verbindung bietet.

- **Akzeptanzkriterien:**
    - Dropdown oder Schnellwahltasten in der UI für den Modus.
    - Der gewählte Modus wird für alle folgenden Messungen gespeichert, bis er geändert wird.
    - Möglichkeit, eigene Kategorien hinzuzufügen.
        

---

### Epic 3: Benachrichtigungen & Alerts

**Story 3.1: Warnung bei Verbindungsabbau**

> **Als** Nutzer **möchte ich** eine Desktop-Benachrichtigung erhalten, sobald die Latenz einen bestimmten Schwellenwert überschreitet oder der Ping fehlschlägt, **damit** ich gewarnt bin, bevor mein Video-Call abbricht oder ich wichtige Arbeit speichere.

- **Akzeptanzkriterien:**
    
    - Schwellenwert ist konfigurierbar (z. B. Ping > 200ms oder 3 Pakete verloren).
    - Benachrichtigung erscheint als Windows Toast Notification.
    - Option, den Alarm stummzuschalten (z. B. "Für 1 Stunde ignorieren").
    - Negative visuelle Rückmeldung (z. B. Rotes Icon).
        

**Story 3.2: Info bei Wiederherstellung**

> **Als** Nutzer **möchte ich** benachrichtigt werden, sobald die Verbindungswerte wieder im normalen Bereich sind, **damit** ich weiß, wann ich meine Online-Tätigkeiten fortsetzen kann, ohne ständig manuell prüfen zu müssen.

- **Akzeptanzkriterien:**
    
    - Die Benachrichtigung erfolgt erst, wenn die Verbindung für eine definierte Zeit (z. B. 30 Sekunden) stabil war (um „Flackern“ zu vermeiden).
    - Positive visuelle Rückmeldung (z. B. Grünes Icon).
        

---

### Epic 4: Datenhistorie & UI

**Story 4.1: Verlaufansicht**

> **Als** Nutzer **möchte ich** eine grafische Übersicht (Chart) der letzten Stunden sehen, **damit** ich erkennen kann, ob Verbindungsprobleme sporadisch oder dauerhaft waren.

- **Akzeptanzkriterien:**
    
    - Liniendiagramm für Ping-Zeiten.
    - Markierung von Verbindungsabbrüchen im Diagramm (rot).
    - Filterung nach Datum/Uhrzeit möglich.
        

**Story 4.2: Datenexport**

> **Als** technisch versierter Nutzer **möchte ich** meine gesammelten Daten als CSV-Datei exportieren können, **damit** ich sie in Excel analysieren oder meinem Internetanbieter als Beweis für schlechte Leistung vorlegen kann.

- **Akzeptanzkriterien:**
    
    - Export-Button in den Einstellungen.
    - CSV enthält: Timestamp, Ping, Download-Rate, Upload-Rate, GPS, Kontext-Tag.
        

---

### Epic 5: Konfiguration

**Story 5.1: Autostart**

> **Als** Nutzer **möchte ich**, dass das Tool beim Start von Windows automatisch im Hintergrund (System Tray) startet, **damit** ich nicht jedes Mal daran denken muss, die Messung zu aktivieren.

- **Akzeptanzkriterien:**
    
    - Option in den Einstellungen "Mit Windows starten".
    - Anwendung minimiert sich beim Start direkt in den Tray.
        

---

Möchtest du diese Stories noch weiter verfeinern (z. B. Designs für das UI diskutieren) oder soll ich dir helfen, eine Architektur für die Windows-App zu entwerfen?
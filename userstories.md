

# Executive Summary: Windows Connectivity Tracker

### Ausgangslage & Problemstellung

In einer zunehmend mobilen und digitalen Arbeitswelt ist eine stabile Internetverbindung die kritischste Ressource. Nutzer � ob im Homeoffice, im Zug oder im Caf� � stehen h�ufig vor dem Problem schwankender Verbindungsqualit�t. Oft ist unklar, ob das Problem beim Provider, am aktuellen Standort oder an der eigenen Auslastung liegt. Es fehlt an Werkzeugen, die diese Daten nicht nur messen, sondern auch mit dem geografischen Standort und dem Nutzungskontext verkn�pfen.

### Projektziel

Ziel dieses Projektes ist die Entwicklung einer nativen Windows-Applikation (�Connectivity Tracker�), die als intelligenter Hintergrunddienst agiert. Das Tool soll Transparenz �ber die Netzqualit�t schaffen, indem es technische Metriken mit realen Welt-Daten (Standort, Umgebung) korreliert und den Nutzer proaktiv bei Ver�nderungen informiert.
### Kernfunktionen (Key Features)

Das System st�tzt sich auf vier funktionale S�ulen:

1. **Echtzeit-Diagnose:** Kontinuierliche �berwachung der Latenz (Ping zu Referenzservern) und des lokalen Datendurchsatzes, um Engp�sse sofort zu identifizieren.
2. **Location Intelligence:** Verkn�pfung von Leistungsdaten mit GPS-Koordinaten, um �Funkl�cher� auf einer Karte visualisierbar zu machen.
3. **Kontext-Bewusstsein:** Manuelle Kategorisierung der Umgebung (z. B. �Im Zug�, �Homeoffice�, �Hotel�), um statistische Vergleiche zwischen verschiedenen Arbeitsorten zu erm�glichen.
4. **Proaktive Warnsysteme:** Intelligente Benachrichtigungen, die den Nutzer warnen, _bevor_ die Verbindung komplett abrei�t (z. B. bei steigender Latenz), und informieren, sobald die Leitung wieder stabil ist.
    

### Nutzenversprechen (Value Proposition)

F�r den Endanwender wandelt das Tool das abstrakte Gef�hl von �schlechtem Internet� in harte Fakten um.

- **Planbarkeit:** Der Nutzer wird gewarnt, wichtige Arbeit zu speichern, wenn die Verbindung instabil wird.
- **Beweisbarkeit:** Historische Daten dienen als Beleg gegen�ber Internetanbietern bei St�rungen.
- **Erkenntnisgewinn:** Der Nutzer lernt, an welchen Orten oder in welchen Verkehrsmitteln produktives Arbeiten m�glich ist.



### Epic 0: UI-Grundger�st & App-Skelett

**Story 0.1: Hauptfenster (Main Shell)**

> **Als** Nutzer **m�chte ich** ein modernes Anwendungsfenster �ffnen k�nnen, das als Rahmen f�r alle zuk�nftigen Inhalte dient, **damit** ich einen zentralen Einstiegspunkt f�r die Nutzung des Tools habe.

- **Akzeptanzkriterien:**
    - Das Fenster verf�gt �ber Standard-Windows-Steuerelemente (Minimieren, Maximieren, Schlie�en).
    - Der Titel der Anwendung wird in der Titelleiste angezeigt.
    - Das Fenster hat eine definierte Mindestgr��e, um Darstellungsfehler zu vermeiden.
    - Das Design nutzt ein modernes Framework (z. B. WPF, WinUI 3 oder .NET MAUI) f�r einen zeitgem��en Look.

**Story 0.2: Navigationsstruktur**

> **Als** Nutzer **m�chte ich** �ber eine Seitenleiste oder Tabs zwischen den verschiedenen Hauptbereichen (Dashboard, Historie, Einstellungen) wechseln k�nnen, **damit** ich sp�ter schnell auf die entsprechenden Funktionen zugreifen kann.

- **Akzeptanzkriterien:**
    - Eine Navigationsleiste (z. B. linksseitig) ist implementiert.
    - Es gibt Platzhalter-Buttons f�r �Dashboard�, �Verlauf� und �Einstellungen�.
    - Beim Klick auf einen Men�punkt wechselt der Hauptinhaltsbereich (Content Region) auf eine leere Platzhalter-Ansicht des jeweiligen Bereichs (Routing funktioniert).
        

**Story 0.3: System Tray Integration (Infobereich)**

> **Als** Nutzer **m�chte ich**, dass sich die Anwendung in den Windows System Tray (Bereich neben der Uhr) minimieren l�sst, anstatt geschlossen zu werden, **damit** das Tool sp�ter unauff�llig im Hintergrund laufen kann, ohne Platz in der Taskleiste zu verbrauchen.

- **Akzeptanzkriterien:**
    
    - Beim Klick auf das �X� (Schlie�en) wird die App nicht beendet, sondern in den Tray minimiert.
    - Es gibt ein App-Icon im System Tray.
    - Ein Doppelklick auf das Tray-Icon �ffnet das Hauptfenster wieder.
    - Ein Rechtsklick auf das Tray-Icon �ffnet ein Kontextmen� mit der Option �Beenden�, um die App vollst�ndig zu schlie�en.
        

**Story 0.4: Dashboard-Layout (Wireframe)**

> **Als** Nutzer **m�chte ich** auf der Startseite (Dashboard) bereits die grobe Aufteilung der Elemente sehen (Platzhalter f�r Graphen, Platzhalter f�r aktuelle Werte, Platzhalter f�r Location-Tag), **damit** die visuelle Hierarchie klar ist, bevor die echte Datenanbindung erfolgt.

- **Akzeptanzkriterien:**
    - Das Layout ist in logische Zonen unterteilt (z. B. "Live-Status" oben gro�, "Verlauf" unten kleiner).
    - Die UI ist "Responsive": Wenn ich das Fenster gr��er ziehe, passen sich die Platzhalter-Boxen sinnvoll an die neue Gr��e an.


### Epic 1: Netzwerk-Monitoring (Ping & Traffic)

**Story 1.1: Regelm��iger Ping**

> **Als** Nutzer **m�chte ich**, dass das Tool in konfigurierbaren Abst�nden automatisch einen Ping an einen zuverl�ssigen Server (z. B. Google DNS) sendet, **damit** ich die aktuelle Latenz (Verz�gerung) meiner Internetverbindung �berwachen kann.

- **Akzeptanzkriterien:**
    - Der Ping erfolgt im Hintergrund ohne Blockieren der UI.
    - Der Nutzer kann das Intervall einstellen (z. B. alle 5, 10 oder 60 Sekunden).
    - Die Antwortzeit (ms) wird gespeichert.
        

**Story 1.2: Traffic-�berwachung**

> **Als** Nutzer **m�chte ich** den aktuellen Datendurchsatz (Upload und Download) meines gesamten Systems in Echtzeit sehen, **damit** ich einsch�tzen kann, ob eine langsame Verbindung an der Leitung oder an meiner eigenen Bandbreitennutzung liegt.

- **Akzeptanzkriterien:**
    - Das Tool liest die Bytes Sent/Received der aktiven Netzwerkkarte aus.
    - Die Anzeige erfolgt in verst�ndlichen Einheiten (KB/s, MB/s).

---

### Epic 2: Standort & Kontext

**Story 2.1: Standort-Tracking**

> **Als** mobiler Nutzer **m�chte ich**, dass meine Verbindungsdaten automatisch mit meinen GPS-Koordinaten (oder Windows Location Service) verkn�pft werden, **damit** ich sp�ter auf einer Karte sehen kann, an welchen geografischen Orten ich Funkl�cher hatte.

- **Akzeptanzkriterien:**
    
    - Abfrage der Windows Location API.
    - Speicherung von L�ngen- und Breitengrad zu jedem Messpunkt.
    - Fallback-Option, falls kein GPS verf�gbar ist (z. B. "Standort unbekannt").
        

**Story 2.2: Kontext-Kategorisierung (Tagging)**

> **Als** Nutzer **m�chte ich** aus vordefinierten Kategorien (Zug, Auto, Zuhause, Caf�, Unterwegs) meinen aktuellen Status ausw�hlen k�nnen, **damit** ich statistisch auswerten kann, welches Verkehrsmittel oder welcher Ort die stabilste Verbindung bietet.

- **Akzeptanzkriterien:**
    - Dropdown oder Schnellwahltasten in der UI f�r den Modus.
    - Der gew�hlte Modus wird f�r alle folgenden Messungen gespeichert, bis er ge�ndert wird.
    - M�glichkeit, eigene Kategorien hinzuzuf�gen.
        

---

### Epic 3: Benachrichtigungen & Alerts

**Story 3.1: Warnung bei Verbindungsabbau**

> **Als** Nutzer **m�chte ich** eine Desktop-Benachrichtigung erhalten, sobald die Latenz einen bestimmten Schwellenwert �berschreitet oder der Ping fehlschl�gt, **damit** ich gewarnt bin, bevor mein Video-Call abbricht oder ich wichtige Arbeit speichere.

- **Akzeptanzkriterien:**
    
    - Schwellenwert ist konfigurierbar (z. B. Ping > 200ms oder 3 Pakete verloren).
    - Benachrichtigung erscheint als Windows Toast Notification.
    - Option, den Alarm stummzuschalten (z. B. "F�r 1 Stunde ignorieren").
    - Negative visuelle R�ckmeldung (z. B. Rotes Icon).

**Story 3.2: Info bei Wiederherstellung**

> **Als** Nutzer **m�chte ich** benachrichtigt werden, sobald die Verbindungswerte wieder im normalen Bereich sind, **damit** ich wei�, wann ich meine Online-T�tigkeiten fortsetzen kann, ohne st�ndig manuell pr�fen zu m�ssen.

- **Akzeptanzkriterien:**
    - Die Benachrichtigung erfolgt erst, wenn die Verbindung f�r eine definierte Zeit (z. B. 30 Sekunden) stabil war (um �Flackern� zu vermeiden).
    - Positive visuelle R�ckmeldung (z. B. Gr�nes Icon).
        

---


**Story 3.3: Ändern des Ping Servers.**
> Als Nutzer möchte ich die möglichkeit haben den server den ich anpinge zu ändern. Ich möchte eine liste von seiten haben die ich anpingen kann. Darunter soll cloudflare, telekom, google, und quad9 sein.
Akzeptanzkriterien:
    - Der Nutzer hat die möglichkeit den Ping Server zu ändern.
    - Der ausgewählte wird persistent gespeichert für den nächsten app start.



**Story 3.4: Icon in der Taskbar**
> Als Nutzer möchte ich die möglichkeit haben den aktuellen Ping in der Taskbar zu sehen. 
Akzeptanzkriterien:
    - in der Taskbar wird der aktuell gemessene Ping angezeigt.


**Story 3.5: Ping im System-Tray (Infobereich) als Icon**
> Als Nutzer möchte ich, dass das System-Tray-Icon (Infobereich) den aktuellen Ping-Wert direkt als Icon darstellt, damit ich den Verbindungszustand schnell ohne zusätzliches Fenster ablesen kann.

Akzeptanzkriterien:
    - Das System-Tray-Icon (NotifyIcon) zeigt den aktuellen Ping-Wert (z. B. "42" oder "X" bei Fehler) als gerendertes 16×16-Icon.
    - Updates erfolgen im konfigurierten Ping-Intervall, mit Throttling (z. B. nicht häufiger als alle 2 Sekunden) um CPU-/I/O-Overhead zu vermeiden.
    - Es gibt eine Settings-Option `Show ping in tray` (Checkbox) — die Einstellung wird persistent gespeichert und beim nächsten Start wiederhergestellt.
    - Bei Systemen oder Umgebungen, in denen das Setzen eines dynamischen Tray-Icon nicht möglich ist, wird auf die Tray-Tooltip-Variante mit aktuellem Ping zurückgegriffen.
    - Darstellung muss bei geringer Icon-Auflösung lesbar bleiben (Kurzformat: `X`, `1K`, oder numerisch gekürzt).
    - Unit-Tests und manuelle Verifikation (Schritte dokumentiert) sind vorhanden.



### Epic 4: Datenhistorie & UI

**Story 4.1: Verlaufansicht**

> **Als** Nutzer **m�chte ich** eine grafische �bersicht (Chart) der letzten Stunden sehen, **damit** ich erkennen kann, ob Verbindungsprobleme sporadisch oder dauerhaft waren.

- **Akzeptanzkriterien:**
    
    - Liniendiagramm f�r Ping-Zeiten.
    - Markierung von Verbindungsabbr�chen im Diagramm (rot).
    - Filterung nach Datum/Uhrzeit m�glich.
        

**Story 4.2: Datenexport**

> **Als** technisch versierter Nutzer **m�chte ich** meine gesammelten Daten als CSV-Datei exportieren k�nnen, **damit** ich sie in Excel analysieren oder meinem Internetanbieter als Beweis f�r schlechte Leistung vorlegen kann.

- **Akzeptanzkriterien:**
    
    - Export-Button in den Einstellungen.
    - CSV enth�lt: Timestamp, Ping, Download-Rate, Upload-Rate, GPS, Kontext-Tag.
        

---

### Epic 5: Konfiguration

**Story 5.1: Autostart**

> **Als** Nutzer **m�chte ich**, dass das Tool beim Start von Windows automatisch im Hintergrund (System Tray) startet, **damit** ich nicht jedes Mal daran denken muss, die Messung zu aktivieren.

- **Akzeptanzkriterien:**
    
    - Option in den Einstellungen "Mit Windows starten".
    - Anwendung minimiert sich beim Start direkt in den Tray.
        

---

M�chtest du diese Stories noch weiter verfeinern (z. B. Designs f�r das UI diskutieren) oder soll ich dir helfen, eine Architektur f�r die Windows-App zu entwerfen?
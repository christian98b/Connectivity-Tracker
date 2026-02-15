# Connectivity Tracker - TODO List

## Epic 0: UI-Grundgerüst & App-Skelett

### Story 0.1: Hauptfenster (Main Shell)
- [x] Create main application window with WPF
- [x] Implement standard Windows controls (Minimize, Maximize, Close)
- [x] Add application title to title bar
- [x] Set minimum window size
- [x] Apply modern design framework styling

### Story 0.2: Navigationsstruktur
- [x] Implement navigation sidebar/tab structure
- [x] Create placeholder buttons for Dashboard
- [x] Create placeholder buttons for History/Verlauf
- [x] Create placeholder buttons for Settings/Einstellungen
- [x] Implement routing between different views
- [x] Test navigation between all sections

### Story 0.3: System Tray Integration
- [x] Implement minimize to system tray instead of close
- [x] Add application icon to system tray
- [x] Implement double-click to restore window from tray
- [x] Create right-click context menu for tray icon
- [x] Add "Exit/Beenden" option in context menu
- [x] Ensure proper cleanup on exit

### Story 0.4: Dashboard-Layout (Wireframe)
- [x] Design dashboard layout with logical zones
- [x] Create placeholder for live status section (top)
- [x] Create placeholder for history graphs (bottom)
- [x] Create placeholder for location tag display
- [x] Implement responsive layout behavior
- [x] Test window resizing and element adaptation

---

## Epic 1: Netzwerk-Monitoring (Ping & Traffic)

### Story 1.1: Regelmäßiger Ping
- [x] Implement background ping functionality
- [x] Configure ping to reliable server (e.g., Google DNS 8.8.8.8)
- [x] Add configurable ping interval (5s, 10s, 60s options)
- [x] Store ping response times (ms) in data structure
- [x] Ensure non-blocking UI during ping operations
- [x] Add error handling for failed pings
- [x] Display current latency in UI

### Story 1.2: Traffic-Überwachung
- [x] Read network adapter statistics (Bytes Sent/Received)
- [x] Calculate upload/download rates
- [x] Convert to readable units (KB/s, MB/s)
- [x] Display real-time traffic in UI
- [x] Update traffic display at regular intervals
- [x] Handle multiple network adapters
- [x] Fix dashboard traffic refresh and idle-speed (`0 B/s`) display behavior

---

## Epic 2: Standort & Kontext

### Story 2.1: Standort-Tracking
- [x] Integrate Windows Location API
- [x] Request location permissions
- [x] Capture GPS coordinates (latitude/longitude)
- [x] Store location data with each measurement
- [x] Implement fallback for unavailable GPS ("Standort unbekannt")
- [x] Test location accuracy and update frequency

### Story 2.2: Kontext-Kategorisierung (Tagging)
- [x] Create predefined categories (Zug, Auto, Zuhause, Café, Unterwegs)
- [x] Implement dropdown/quick select UI for context selection
- [x] Store selected context with measurements
- [x] Persist context selection until manually changed
- [x] Add ability to create custom categories
- [x] Display current context in UI

---

## Epic 3: Benachrichtigungen & Alerts

### Story 3.1: Warnung bei Verbindungsabbau
- [x] Define configurable alert thresholds (e.g., Ping > 200ms)
- [x] Implement Windows Toast Notification system
- [x] Trigger alert when threshold exceeded
- [x] Add red icon/visual indicator for poor connection
- [x] Implement "Snooze" functionality (ignore for X minutes)
- [x] Add settings UI for threshold configuration
- [x] Test notification reliability

### Story 3.2: Info bei Wiederherstellung
- [x] Detect when connection returns to normal range
- [x] Implement stability check (30s stable before notification)
- [x] Send recovery notification
- [x] Add green icon/visual indicator for good connection
- [x] Prevent notification "flicker" during unstable periods

---

## Epic 4: Datenhistorie & UI

### Story 4.1: Verlaufansicht
- [x] Choose and integrate charting library
- [x] Implement line chart for ping times
- [x] Add red markers for connection failures
- [x] Implement date/time filtering
- [x] Add zoom and pan functionality
- [x] Display historical traffic data
- [x] Add legend and axis labels

### Story 4.2: Datenexport
- [ ] Create export button in settings
- [ ] Implement CSV export functionality
- [ ] Include all data fields: Timestamp, Ping, Download, Upload, GPS, Context
- [ ] Add file save dialog
- [ ] Test CSV format with Excel/other tools
- [ ] Add date range selection for export

---

## Epic 5: Konfiguration

### Story 5.1: Autostart
- [ ] Create settings UI for autostart option
- [ ] Implement Windows startup registry entry
- [ ] Add "Start with Windows" checkbox
- [ ] Configure app to minimize to tray on startup
- [ ] Test autostart functionality
- [ ] Add option to disable autostart

---

## Additional Technical Tasks

### Data Persistence
- [x] Choose database solution (SQLite recommended)
- [x] Design database schema
- [x] Implement data access layer
- [x] Add automatic data cleanup (old records)
- [ ] Implement data migration strategy

### Testing & Quality
- [ ] Unit tests for core functionality
- [ ] Integration tests for network monitoring
- [ ] UI automation tests
- [ ] Performance testing
- [ ] Memory leak testing

### Documentation
- [ ] User manual/help documentation
- [ ] Installation guide
- [ ] Developer documentation
- [x] README file
- [ ] License information

### Deployment
- [ ] Create installer package
- [ ] Code signing certificate
- [ ] Version management
- [ ] Update mechanism
- [ ] Distribution strategy

---

## Notes
- Priority focus: Epic 0 and Epic 1 (Core functionality)
- Consider using async/await for all network operations
- Ensure proper resource cleanup and memory management
- Plan for localization (German/English)
